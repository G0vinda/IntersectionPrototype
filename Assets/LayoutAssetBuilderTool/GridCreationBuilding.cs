using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LayoutAssetBuilderTool
{   
    public class GridCreationBuilding : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite waterSprite;
        [SerializeField] private Sprite parkSprite;
        [SerializeField] private Image image;

        private GridCreationTool _gridCreationTool;
        private Dictionary<State, Sprite> _sprites;
        private Vector2Int _coordinates;
        private State _currentState;

        public void Initialize(GridCreationTool gridCreationTool, Vector2Int coordinates, int state)
        {
            _gridCreationTool = gridCreationTool;
            _coordinates = coordinates;

            _sprites = new Dictionary<State, Sprite>()
            {
                {State.Normal, normalSprite},
                {State.Water, waterSprite},
                {State.Park, parkSprite}
            };

            _currentState = (State)state;
            UpdateAppearance();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _gridCreationTool.BuildingClicked(_coordinates);
        }

        public void SetWaterState(bool isWater)
        {
            _currentState = isWater ? State.Water : State.Normal;
            UpdateAppearance();
        }

        public void SetParkState(bool isPark)
        {
            _currentState = isPark ? State.Park : State.Normal;
            UpdateAppearance();
        }

        private void UpdateAppearance()
        {
            image.sprite = _sprites[_currentState];
        }

        public enum State{
            Normal,
            Water,
            Park
        }
    }
}
