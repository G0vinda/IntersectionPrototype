using System.Collections.Generic;
using UnityEngine;

namespace GridCreationTool
{
    public class LayoutFieldContainer : MonoBehaviour
    {
        [SerializeField] private LayoutUIField layoutUIFieldPrefab;

        public void SetupLayouts(List<LayoutAssetBuilderTool.GridCreationTool.LayoutBlockData> layoutData)
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
