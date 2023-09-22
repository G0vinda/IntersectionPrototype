using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CityGridCreator cityGrid;
    [SerializeField] private Vector2Int startCoordinates;
    [SerializeField] private float characterYOffset;

    private Vector2Int _currentCoordinates;
    private Vector3 _characterOffset;
    
    private void Start()
    {
        _characterOffset = Vector3.up * characterYOffset;
        _currentCoordinates = startCoordinates;
        transform.position = cityGrid.GetIntersectionPosition(startCoordinates) + _characterOffset;
    }

    public void MovePlayer(Vector2Int direction)
    {
        _currentCoordinates += direction;
        var destination = cityGrid.GetIntersectionPosition(_currentCoordinates) + _characterOffset;

        transform.position = destination;
    }
}
