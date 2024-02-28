using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IBuildingGroupable
{
    [SerializeField] MeshRenderer topRenderer;

    public void SetMaterial(Material newMaterial)
    {
        topRenderer.material = newMaterial;
    }
}
