using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkDeko : MonoBehaviour
{
    [SerializeField] GameObject[] dekoPrefabs;

    void Awake()
    {
        var randomPrefab = dekoPrefabs[Random.Range(0, dekoPrefabs.Length)];
        var dekoObject = Instantiate(randomPrefab, transform);
        dekoObject.transform.rotation = Quaternion.identity;
    }
}
