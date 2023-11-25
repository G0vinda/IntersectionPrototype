using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private float characterYOffset;
        [SerializeField] private float moveTime;
        [SerializeField] private float lookAheadInputTime;
        [SerializeField] private float npcPushSpeed;
        
        private ScoringSystem _scoringSystem;
        private CityGridCreator _cityGrid;
        private Vector2Int _currentCoordinates;
        private Vector3 _characterOffset;
        private Tween _moveTween;
        private Vector2Int? _queuedMoveInput;
        private bool _openForLookAheadInput;
        private Collider _collider;

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

        public void Initialize(Vector3 startPosition, Vector2Int startCoordinates, CityGridCreator cityGrid, ScoringSystem scoringSystem)
        {
            _characterOffset = Vector3.up * characterYOffset;
            _scoringSystem = scoringSystem;
            _currentCoordinates = startCoordinates;
            transform.position = startPosition + _characterOffset;
            _cityGrid = cityGrid;
            _collider = GetComponent<Collider>();
        }

        private void MovePlayer(Vector2Int direction)
        {
            if (_moveTween != null)
            {
                if (_openForLookAheadInput && _queuedMoveInput == null)
                    _queuedMoveInput = direction;
                
                return;    
            }

            StartCoroutine(PrepareForLookAheadInput(moveTime - lookAheadInputTime));

            if (!_cityGrid.TryGetIntersectionPosition(_currentCoordinates + direction, out var destination))
                return;

            _currentCoordinates += direction;
            if (direction == Vector2Int.up)
                _cityGrid.GenerateNextRowInFront();

            _moveTween = transform.DOMove(destination + _characterOffset, moveTime).SetEase(Ease.OutSine).OnComplete(() =>
            {
                if (direction == Vector2Int.up)
                    _scoringSystem.IncrementScore();
                
                _moveTween = null;
                if (_queuedMoveInput != null)
                {
                    MovePlayer(_queuedMoveInput.Value);
                }
            });
        }

        public void PushPlayerBackTunnel()
        {
            _moveTween?.Kill();
            _queuedMoveInput = null;

            _currentCoordinates += Vector2Int.down;

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

        public void PushPlayerByNpc(int amount) // negative amount pushes back, positive forwards...
        {
            _moveTween?.Kill();
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

            StartCoroutine(NpcPush(pushPosition + new Vector3(0, characterYOffset, 0), Math.Abs(amount) * npcPushSpeed));
            _currentCoordinates = pushCoordinates;
            _scoringSystem.ChangeScore(amount);
        }

        private IEnumerator NpcPush(Vector3 pushDestination, float pushTime)
        {
            _openForLookAheadInput = false;
            var startPosition = transform.position;
            var startHeight = transform.position.y;
            var maxHeight = 30f;
            var timer = 0f;
            var verticalVelocity = Vector3.zero;
            _collider.enabled = false;

            do
            {
                timer += Time.deltaTime;
                var newPosition = Vector3.Lerp(startPosition, pushDestination, timer / pushTime);
                transform.position = newPosition;
                
                yield return null;
            } while (timer < pushTime);
            
            _collider.enabled = true;
            _moveTween = null;
            _openForLookAheadInput = true;
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
