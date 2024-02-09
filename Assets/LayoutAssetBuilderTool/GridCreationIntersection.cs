using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LayoutAssetBuilderTool
{
    public class GridCreationIntersection : MonoBehaviour, IPointerDownHandler
    {
        private GridCreationTool _gridCreationTool;
        private Image _image;
        private Vector2Int _coordinates;
        
        public void Initialize(GridCreationTool gridCreationTool, Vector2Int coordinates, int maxX, int maxY)
        {
            _image = GetComponent<Image>();
            _gridCreationTool = gridCreationTool;
            _coordinates = coordinates;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _gridCreationTool.IntersectionClicked(_coordinates);
        }
    }
}