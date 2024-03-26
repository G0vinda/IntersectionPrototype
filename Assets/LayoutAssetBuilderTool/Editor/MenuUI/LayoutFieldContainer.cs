using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
{
    public class LayoutFieldContainer : MonoBehaviour
    {
        [SerializeField] private LayoutUIField layoutUIFieldPrefab;

        public Transform[] SetupLayouts(List<CityLayout.LayoutBlockData> layoutData)
        {
            DestroyChildren();
            
            if(layoutData == null)
                return null;
            
            var transforms = new List<Transform>();
            for (var i = 0; i < layoutData.Count; i++)
            {
                var newLayoutUIField = Instantiate(layoutUIFieldPrefab, transform);
                var layout = layoutData.ElementAt(i);
                newLayoutUIField.Initialize(layout.id, layout.name, layout.difficulty);
                transforms.Add(newLayoutUIField.transform);
            }

            return transforms.ToArray();
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