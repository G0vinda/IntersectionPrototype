using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CityGridCreator cityGrid;
    [SerializeField] private Vector2Int startCoordinates;
    [SerializeField] private float characterYOffset;
    [SerializeField] private float moveTime;

    private Vector2Int _currentCoordinates;
    private Vector3 _characterOffset;
    private Tween _moveTween;
    
    private void Start()
    {
        _characterOffset = Vector3.up * characterYOffset;
        _currentCoordinates = startCoordinates;
        cityGrid.TryGetIntersectionPosition(startCoordinates, out var startPosition);
        transform.position = startPosition + _characterOffset;
    }

    public void MovePlayer(Vector2Int direction)
    {
        if(_moveTween != null)
            return;

        if (!cityGrid.TryGetIntersectionPosition(_currentCoordinates + direction, out var destination))
            return;

        _currentCoordinates += direction;
        if (direction == Vector2Int.up)
            cityGrid.GenerateNextRow();

        _moveTween = transform.DOMove(destination + _characterOffset, moveTime).SetEase(Ease.OutSine).OnComplete(() =>
        {
            _moveTween = null;
        });
    }
}
