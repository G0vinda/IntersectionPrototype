using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGroup : MonoBehaviour
{
    [SerializeField] Material[] possibleMaterials;

    private Material _groupMaterial;
    
    public void TransferChildrenTo(BuildingGroup newParent)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            newParent.AssignObject(child.gameObject.GetComponent<IBuildingGroupable>());
        }
        Destroy(gameObject);
    }

    public void AssignObject(IBuildingGroupable newObject)
    {
        if(_groupMaterial == null)
            SetRandomMaterial();
        
        newObject.SetMaterial(_groupMaterial);
        var mb = newObject as MonoBehaviour;
        mb.transform.SetParent(transform);
    }

    private void SetRandomMaterial()
    {
        _groupMaterial = possibleMaterials[Random.Range(0, possibleMaterials.Length)];
    }
}
