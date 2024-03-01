using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentDeko : MonoBehaviour
{
    [SerializeField] GameObject[] dekoVariants;
    [Range(0, 1f)]
    [SerializeField] float dekoProbability;

    void Awake()
    {
        for (int i = 0; i < dekoVariants.Length; i++)
        {
            dekoVariants[i].SetActive(false);
        }

        if(Random.Range(0, 1f) <= dekoProbability)
        {
            var variantIndex = Random.Range(0, dekoVariants.Length);
            dekoVariants[variantIndex].SetActive(true);
        }
    }
}
