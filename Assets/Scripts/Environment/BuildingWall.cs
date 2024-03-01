using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingWall : MonoBehaviour
{
    [SerializeField] GameObject[] wallVariants;

    void Awake()
    {
        var variantIndex = Random.Range(0, wallVariants.Length);
        for (int i = 0; i < wallVariants.Length; i++)
        {
            wallVariants[i].SetActive(variantIndex == i);
        }
    }
}
