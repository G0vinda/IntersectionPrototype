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

    private void Start()
    {
        var halfCityBlockDistance = cityBlockDistance / 2.0f;
        for (var x = 0; x < gridXSize; x++)
        {
            for (var y = 0; y < gridYSize; y++)
            {
                var intersectionPosition = new Vector3(x * cityBlockDistance, 0 , y * cityBlockDistance);
                _cityGrid[new Vector2Int(x, y)] =
                    Instantiate(intersectionPrefab, intersectionPosition, Quaternion.identity, transform);
                
                if (x == 0)
                {
                    var firstCityBlockPosition = intersectionPosition + new Vector3(-halfCityBlockDistance, 0, -halfCityBlockDistance);
                    Instantiate(GetRandomCityBlockPrefab(), firstCityBlockPosition, Quaternion.identity, transform);
                }

                var cityBlockPosition =
                    intersectionPosition + new Vector3(halfCityBlockDistance, 0, -halfCityBlockDistance);
                Instantiate(GetRandomCityBlockPrefab(), cityBlockPosition, Quaternion.identity, transform);
            }
        }
    }

    private GameObject GetRandomCityBlockPrefab()
    {
        var randomIndex = Random.Range(0, cityBlockPrefabs.Length);
        return cityBlockPrefabs[randomIndex];
    }

    public Vector3 GetIntersectionPosition(Vector2Int coordinates)
    {
        return _cityGrid[coordinates].transform.position;
    }
}
