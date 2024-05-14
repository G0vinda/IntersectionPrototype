using System;
using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace Character
{
    public class PlayerMovement : CharacterMovement
    {
        [SerializeField] private float lookAheadInputTime;
        [SerializeField] private float npcPushSpeed;
        [SerializeField] private float invincibilityTime;
        [SerializeField] private ParticleSystem parkParticles;
        
        public ParticleSystem wallCollisionParticlePrefab;

        private ScoringSystem _scoringSystem;
        private CityGridCreator _cityGrid;
        private Vector2Int _currentCoordinates;
        private Vector3 _characterOffset;
        private Vector2Int _moveDirection;
        private Vector3 _moveDestination;
        private bool _characterControlEnabled = true;
        private Vector2Int? _queuedMoveInput;
        private bool _openForLookAheadInput;
        private Collider _collider;
        private bool _invincible;
        private CharacterAppearance _characterAppearance;
        private WaitForSeconds _invincibilityWait;

        #region OnEnable/OnDisable

        private void OnEnable()
        {
            InputManager.RegisteredMoveInput += MovePlayer;
        }

        private void OnDisable()
        {
            InputManager.RegisteredMoveInput -= MovePlayer;
        }

        #endregion

        public void Initialize(Vector2Int startCoordinates, CityGridCreator cityGrid,
            ScoringSystem scoringSystem)
        {
            _characterOffset = Vector3.up * YOffset;
            _scoringSystem = scoringSystem;
            _currentCoordinates = startCoordinates;
            _cityGrid = cityGrid;
            _cityGrid.TryGetIntersectionPosition(startCoordinates, out var startPosition);
            transform.position = startPosition + _characterOffset;
            _collider = GetComponent<Collider>();
            _characterAppearance = GetComponent<CharacterAppearance>();
            _invincibilityWait = new WaitForSeconds(invincibilityTime);
        }

        public void SetCoordinates(Vector2Int coordinates, bool asInvincible)
        {
            _currentCoordinates = coordinates;
            _cityGrid.TryGetIntersectionPosition(_currentCoordinates, out var newPosition);
            transform.position = newPosition + _characterOffset;

            if(asInvincible)
                StartInvincibility();
        }

        private void MovePlayer(Vector2Int direction)
        {
            if (!_characterControlEnabled)
                return;

            if (_moveTween != null)
            {
                if (_openForLookAheadInput && _queuedMoveInput == null)
                    _queuedMoveInput = direction;

                return;
            }

            StartCoroutine(PrepareForLookAheadInput(moveTime - lookAheadInputTime));

            _cityGrid.TryGetIntersectionPosition(_currentCoordinates + direction, out var destination);
            _moveDirection = direction;
            _moveDestination = destination + _characterOffset;

            var movesHorizontal = _moveDirection == Vector2Int.left || _moveDirection == Vector2Int.right;
            Move(destination, movesHorizontal, () => AfterMove(direction));
        }

        public void PushPlayerBackObstacle()
        {
            _moveTween?.Kill();
            _queuedMoveInput = null;
            streetParticles.Stop();
            parkParticles.Stop();
            SetAnimationToIdle();

            if (!_cityGrid.TryGetIntersectionPosition(_currentCoordinates, out var intersectionPosition))
                throw new Exception("Intersection at Coordinates not found.");

            _openForLookAheadInput = false;
            var pushPosition = intersectionPosition + new Vector3(0, YOffset, 0);
            _moveTween = transform.DOMove(pushPosition, moveTime).SetEase(Ease.InBack).OnComplete(() =>
            {
                _moveTween = null;
                _openForLookAheadInput = true;
            });
        }

        public bool RequestPushByNpc()
        {
            if (_invincible)
                return false;

            _moveTween?.Kill();
            SetAnimationToIdle();
            _characterControlEnabled = false;
            _openForLookAheadInput = false;
            return true;
        }

        // should only be called after "RequestPushByNpc"
        // negative amount pushes back, positive forwards...
        public void PushPlayerByNpc(int amount)
        {
            if (_invincible)
            {
                Debug.LogError("Npc tried to push invincible Player!");
                return;
            }

            streetParticles.Stop();
            parkParticles.Stop();

            _invincible = true;

            var pushCoordinates = _currentCoordinates;
            pushCoordinates.y += amount;

            _cityGrid.TryGetIntersectionPosition(pushCoordinates, out var pushPosition);

            StartCoroutine(PerformNpcPush(pushPosition + new Vector3(0, YOffset, 0),
                Math.Abs(amount) * npcPushSpeed));
            _currentCoordinates = pushCoordinates;
            _scoringSystem.ChangeScore(amount);
        }

        public void SlowCharacter()
        {
            var slowFactor = 2f;
            if(_moveTween == null)
                return;

            // Todo: check if just got pushed by npc (e.g. is invincible)

            var remainingMoveTime = _moveTween.Duration() - _moveTween.Elapsed();        
            _moveTween.Kill();

            streetParticles.Stop();
            parkParticles.Play();

            _moveTween = transform.DOMove(_moveDestination, remainingMoveTime * slowFactor).SetEase(Ease.OutSine).OnComplete(
                () => AfterMove(_moveDirection));
        }

        public void IncrementScore()
        {
            _scoringSystem.IncrementScore();
        }

        private IEnumerator PerformNpcPush(Vector3 pushDestination, float pushTime)
        {
            var startPosition = transform.position;
            var timer = 0f;
            _collider.enabled = false;

            do
            {
                timer += Time.deltaTime;
                var newPosition = Vector3.Lerp(startPosition, pushDestination, timer / pushTime);
                transform.position = newPosition;

                yield return null;
            } while (timer < pushTime);

            StartCoroutine(StartInvincibility());
        }

        private IEnumerator StartInvincibility()
        {
            _invincible = true;
            _characterAppearance.StartInvincibilityBlinking();
            _collider.enabled = true;
            _moveTween = null;
            _characterControlEnabled = true;
            _openForLookAheadInput = true;

            yield return _invincibilityWait;
            _characterAppearance.StopInvincibilityBlinking();
            _invincible = false;
        }

        private void AfterMove(Vector2Int direction)
        {
            _currentCoordinates += direction;
            if (direction == Vector2Int.up)
                _scoringSystem.IncrementScore();
            
            if(direction == Vector2Int.down)
                _scoringSystem.DecrementScore();

            parkParticles.Stop();
            streetParticles.Stop();

            _moveTween = null;
            if (_queuedMoveInput != null)
            {
                MovePlayer(_queuedMoveInput.Value);
            }
            else
            {
                SetAnimationToIdle();
            }
        }

        private IEnumerator PrepareForLookAheadInput(float waitTime)
        {
            _queuedMoveInput = null;
            _openForLookAheadInput = false;
            yield return new WaitForSeconds(waitTime);

            _openForLookAheadInput = true;
        }
    }
}