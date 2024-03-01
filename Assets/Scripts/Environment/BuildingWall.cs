using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingWall : MonoBehaviour
{
    [SerializeField] GameObject[] wallVariants;
    [Range(0, 1f)]
    [SerializeField] float wallProbability;

    void Awake()
    {
        for (int i = 0; i < wallVariants.Length; i++)
        {
            wallVariants[i].SetActive(false);
        }

        if(Random.Range(0, 1f) <= wallProbability)
        {
            var variantIndex = Random.Range(0, wallVariants.Length);
            wallVariants[variantIndex].SetActive(true);
        }
    }
}
