using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-1)]
public class CityGridCreator : MonoBehaviour
{
    [SerializeField] private int gridXSize;
    [SerializeField] private int gridYSize;
    [SerializeField] private float cityBlockDistance;

    [SerializeField] private GameObject[] cityBlockPrefabs;
    [SerializeField] private GameObject intersectionPrefab;

    private Dictionary<Vector2Int, GameObject> _cityGrid = new ();
    private float _halfCityBlockDistance;
    private int _currentYLevel;

    private void Start()
    {
        _halfCityBlockDistance = cityBlockDistance / 2.0f;
        
        for (var y = 0; y < gridYSize; y++)
        {
            GenerateNextRow();
        }
    }

    private GameObject GetRandomCityBlockPrefab()
    {
        var randomIndex = Random.Range(0, cityBlockPrefabs.Length);
        return cityBlockPrefabs[randomIndex];
    }

    public bool TryGetIntersectionPosition(Vector2Int coordinates, out Vector3 intersectionPosition)
    {
        intersectionPosition = Vector3.negativeInfinity;
        if (!_cityGrid.ContainsKey(coordinates))
            return false;
        
        intersectionPosition = _cityGrid[coordinates].transform.position;
        return true;
    }

    public void GenerateNextRow()
    {
        for (var x = 0; x < gridXSize; x++)
        {
            var intersectionPosition = new Vector3(x * cityBlockDistance, 0 , _currentYLevel * cityBlockDistance);
            _cityGrid[new Vector2Int(x, _currentYLevel)] =
                Instantiate(intersectionPrefab, intersectionPosition, Quaternion.identity, transform);
                
            if (x == 0)
            {
                var firstCityBlockPosition = intersectionPosition + new Vector3(-_halfCityBlockDistance, 0, -_halfCityBlockDistance);
                Instantiate(GetRandomCityBlockPrefab(), firstCityBlockPosition, Quaternion.identity, transform);
            }

            var cityBlockPosition =
                intersectionPosition + new Vector3(_halfCityBlockDistance, 0, -_halfCityBlockDistance);
            Instantiate(GetRandomCityBlockPrefab(), cityBlockPosition, Quaternion.identity, transform);
        }
        
        _currentYLevel++;
    }

    public void GenerateRows(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            GenerateNextRow();
        }
    }
}
