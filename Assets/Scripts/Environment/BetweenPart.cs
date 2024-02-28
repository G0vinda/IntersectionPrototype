using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetweenPart : MonoBehaviour, IBuildingGroupable
{
    [SerializeField] MeshRenderer topRenderer;

    public void SetMaterial(Material newMaterial)
    {
        topRenderer.material = newMaterial;    
    }
}
