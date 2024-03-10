using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
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
                newLayoutUIField.Initialize(layoutData.ElementAt(i).id, layoutData.ElementAt(i).name);
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