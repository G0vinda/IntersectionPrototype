using System;
using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace Character
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private float characterYOffset;
        [SerializeField] private float moveTime;
        [SerializeField] private float lookAheadInputTime;
        [SerializeField] private float npcPushSpeed;
        [SerializeField] private float invincibilityTime;

        private ScoringSystem _scoringSystem;
        private CityGridCreator _cityGrid;
        private Vector2Int _currentCoordinates;
        private Vector3 _characterOffset;
        private Tween _moveTween;
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

        public void Initialize(Vector3 startPosition, Vector2Int startCoordinates, CityGridCreator cityGrid,
            ScoringSystem scoringSystem)
        {
            _characterOffset = Vector3.up * characterYOffset;
            _scoringSystem = scoringSystem;
            _currentCoordinates = startCoordinates;
            transform.position = startPosition + _characterOffset;
            _cityGrid = cityGrid;
            _collider = GetComponent<Collider>();
            _characterAppearance = GetComponent<CharacterAppearance>();
            _invincibilityWait = new WaitForSeconds(invincibilityTime);
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

            if (!_cityGrid.TryGetIntersectionPosition(_currentCoordinates + direction, out var destination))
                return;
            
            if (direction == Vector2Int.up)
                 _cityGrid.PrepareRowsAhead(_currentCoordinates.y + 1);

            _moveTween = transform.DOMove(destination + _characterOffset, moveTime).SetEase(Ease.OutSine).OnComplete(
                () =>
                {
                    _currentCoordinates += direction;
                    if (direction == Vector2Int.up)
                        _scoringSystem.IncrementScore();

                    _moveTween = null;
                    if (_queuedMoveInput != null)
                    {
                        MovePlayer(_queuedMoveInput.Value);
                    }
                });
        }

        public void PushPlayerBackObstacle()
        {
            _moveTween?.Kill();
            _queuedMoveInput = null;

            if (!_cityGrid.TryGetIntersectionPosition(_currentCoordinates, out var intersectionPosition))
                throw new Exception("Intersection at Coordinates not found.");

            _openForLookAheadInput = false;
            var pushPosition = intersectionPosition + new Vector3(0, characterYOffset, 0);
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

            _invincible = true;

            var pushCoordinates = _currentCoordinates;
            pushCoordinates.y += amount;
            if (amount > 0)
            {
                _cityGrid.PrepareRowsAhead(pushCoordinates.y);
            }
            else
            {
                _cityGrid.PrepareRowsInBack(pushCoordinates.y);
            }

            _cityGrid.TryGetIntersectionPosition(pushCoordinates, out var pushPosition);

            StartCoroutine(PerformNpcPush(pushPosition + new Vector3(0, characterYOffset, 0),
                Math.Abs(amount) * npcPushSpeed));
            _currentCoordinates = pushCoordinates;
            _scoringSystem.ChangeScore(amount);
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

            _characterAppearance.StartInvincibilityBlinking();
            _collider.enabled = true;
            _moveTween = null;
            _characterControlEnabled = true;
            _openForLookAheadInput = true;

            yield return _invincibilityWait;
            _characterAppearance.StopInvincibilityBlinking();
            _invincible = false;
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