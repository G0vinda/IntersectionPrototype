using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LayoutAssetBuilderTool
{
    public class GridCreationIntersection : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite waterSprite;

        private GridCreationTool _gridCreationTool;
        private Vector2Int _coordinates;
        private State _currentState;
        
        public void Initialize(GridCreationTool gridCreationTool, Vector2Int coordinates, int state)
        {
            image = GetComponent<Image>();
            _gridCreationTool = gridCreationTool;
            _coordinates = coordinates;
            _currentState = (State)state;
            UpdateAppearance();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _gridCreationTool.IntersectionClicked(_coordinates);
        }

        public void SetWaterState(bool isWater)
        {
            _currentState = isWater ? State.Water : State.Normal;
            UpdateAppearance();
        }

        private void UpdateAppearance()
        {
            image.sprite = _currentState == State.Water ? waterSprite : normalSprite;
        }

        public enum State{
            Normal,
            Water
        }
    }
}