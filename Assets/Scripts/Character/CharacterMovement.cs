using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Character
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private float characterYOffset;
        [SerializeField] private float moveTime;
        [SerializeField] private float lookAheadInputTime;

        private ScoringSystem _scoringSystem;
        private CityGridCreator _cityGrid;
        private Vector2Int _currentCoordinates;
        private Vector3 _characterOffset;
        private Tween _moveTween;
        private Vector2Int? _queuedMoveInput;
        private bool _openForLookAheadInput;

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
            {
                _cityGrid.GenerateNextRow();
                _scoringSystem.IncrementScore();
            }

            _moveTween = transform.DOMove(destination + _characterOffset, moveTime).SetEase(Ease.OutSine).OnComplete(() =>
            {
                _moveTween = null;
                if (_queuedMoveInput != null)
                {
                    MovePlayer(_queuedMoveInput.Value);
                }
            });
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
