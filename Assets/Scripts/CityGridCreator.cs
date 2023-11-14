using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CityGridCreator : MonoBehaviour
{
    [SerializeField] private int gridXSize;
    [SerializeField] private int gridYSize;
    [SerializeField] private float cityBlockDistance;

    [FormerlySerializedAs("cityBlockPrefabs")] 
    [SerializeField] private GameObject cityBlockPrefab;
    [SerializeField] private GameObject intersectionPrefab;
    [FormerlySerializedAs("street")] [SerializeField] private GameObject streetPrefab;

    [SerializeField] private CharacterAppearance npcPrefab;
    [SerializeField] private float npcPopulationProbability;

    private Dictionary<Vector2Int, GameObject> _cityGrid = new ();
    private float _halfCityBlockDistance;
    private int _currentMaxYLevel;

    public void CreateNewCityGrid()
    {
        _currentMaxYLevel = 0;
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        
        _halfCityBlockDistance = cityBlockDistance / 2.0f;
        
        for (var y = 0; y < gridYSize; y++)
        {
            GenerateNextRow(y != 0);
        }
    }

    public bool TryGetIntersectionPosition(Vector2Int coordinates, out Vector3 intersectionPosition)
    {
        intersectionPosition = Vector3.negativeInfinity;
        if (!_cityGrid.ContainsKey(coordinates))
            return false;
        
        intersectionPosition = _cityGrid[coordinates].transform.position;
        return true;
    }

    public (int, int) GetCurrentXBounds()
    {
        return (0, gridXSize - 1);
    }

    public (int, int) GetCurrentYBounds()
    {
        return (_currentMaxYLevel - gridYSize, _currentMaxYLevel);
    }

    public void GenerateNextRow(bool withNpc = true)
    {
        for (var x = 0; x < gridXSize; x++)
        {
            var intersectionPosition = new Vector3(x * cityBlockDistance, 0 , _currentMaxYLevel * cityBlockDistance);
            _cityGrid[new Vector2Int(x, _currentMaxYLevel)] =
                Instantiate(intersectionPrefab, intersectionPosition, Quaternion.identity, transform);
                
            if (x == 0)
            {
                var firstCityBlockPosition = intersectionPosition + new Vector3(-_halfCityBlockDistance, 0, -_halfCityBlockDistance);
                Instantiate(cityBlockPrefab, firstCityBlockPosition, Quaternion.identity, transform);
                Instantiate(streetPrefab, firstCityBlockPosition + new Vector3(0, 0, -8), Quaternion.identity,
                    transform);
                Instantiate(streetPrefab, firstCityBlockPosition + new Vector3(0, 0, 8), Quaternion.identity,
                    transform);
                Instantiate(streetPrefab, firstCityBlockPosition + new Vector3(-8, 0, 0), Quaternion.Euler(0, -90, 0),
                    transform);
            }

            var cityBlockPosition =
                intersectionPosition + new Vector3(_halfCityBlockDistance, 0, -_halfCityBlockDistance);
            Instantiate(cityBlockPrefab, cityBlockPosition, Quaternion.identity, transform);
            Instantiate(streetPrefab, cityBlockPosition + new Vector3(0, 0, 8), Quaternion.identity, transform);
            Instantiate(streetPrefab, cityBlockPosition + new Vector3(-8, 0, 0), Quaternion.Euler(0, -90, 0),
                transform);
            
            if (withNpc)
            {
                var newNpc = GenerateNpc();
                if (newNpc != null)
                {
                    newNpc.position = intersectionPosition + new Vector3(0, 0.5f, 0);
                }
            }
        }
        
        _currentMaxYLevel++;
    }

    private Transform GenerateNpc()
    {
        var random = Random.Range(0, 1.0f);
        if (random > npcPopulationProbability)
            return null;

        var newNpc = Instantiate(npcPrefab, transform);
        newNpc.Initialize();
        newNpc.SetAppearance(Random.Range(0, newNpc.GetShapesLength()), Random.Range(0, newNpc.GetColorLength()));

        return newNpc.transform;
    }

    public void GenerateRows(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            GenerateNextRow();
        }
    }
}
