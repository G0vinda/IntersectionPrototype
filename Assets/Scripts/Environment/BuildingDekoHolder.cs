using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDekoHolder : MonoBehaviour
{
    [SerializeField] [Range(0, 1f)] private float spawnProbability;
    [SerializeField] GameObject[] dekoAssetPrefabs;

    [Header("AllowedRotations")]
    [SerializeField] bool rot_90;
    [SerializeField] bool rot_180;
    [SerializeField] bool rot_270;

    void Start()
    {
        if(Random.Range(0, 1f) > spawnProbability)
            return;
        
        var prefabToSpawn = dekoAssetPrefabs[Random.Range(0, dekoAssetPrefabs.Length - 1)];
        var deko = Instantiate(prefabToSpawn, transform.position, transform.rotation, transform);
        
        int rotationIndex;
        var rotationAllowed = false;
        Quaternion rotation = Quaternion.identity;
        do
        {
            rotationIndex = Random.Range(0, 3);
            switch (rotationIndex)
            {
                case 0:
                    rotation = Quaternion.identity;
                    rotationAllowed = true;
                    break;
                case 1:
                    rotation = Quaternion.Euler(0, 90, 0);
                    rotationAllowed = rot_90;
                    break;
                case 2:
                    rotation = Quaternion.Euler(0, 180, 0);
                    rotationAllowed = rot_180;
                    break;
                case 3:
                    rotation = Quaternion.Euler(0, 270, 0);
                    rotationAllowed = rot_270;
                    break;
            }
        } while (!rotationAllowed);

        deko.transform.localRotation = rotation;
    }
}
