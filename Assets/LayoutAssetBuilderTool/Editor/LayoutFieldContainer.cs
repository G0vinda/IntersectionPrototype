using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

namespace GridCreationTool
{
    public class LayoutFieldContainer : MonoBehaviour
    {
        [SerializeField] private LayoutUIField layoutUIFieldPrefab;

        public void SetupLayouts(List<CityLayout.LayoutBlockData> layoutData)
        {
            DestroyChildren();
            
            if(layoutData == null)
                return;
            
            for (var i = 0; i < layoutData.Count; i++)
            {
                var newLayoutUIField = Instantiate(layoutUIFieldPrefab, transform);
                newLayoutUIField.Initialize(i);
            }
        }

        private void DestroyChildren()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
    
}

#endif