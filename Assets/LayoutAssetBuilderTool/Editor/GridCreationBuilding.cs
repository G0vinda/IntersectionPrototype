using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
{   
    public class GridCreationBuilding : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite waterSprite;
        [SerializeField] private Sprite parkSprite;
        [SerializeField] private Image image;

        private GridCreationTool _gridCreationTool;
        private Dictionary<CityLayout.BuildingType, Sprite> _sprites;
        private Vector2Int _coordinates;
        private CityLayout.BuildingType _currentState;

        public void Initialize(GridCreationTool gridCreationTool, Vector2Int coordinates, int state)
        {
            _gridCreationTool = gridCreationTool;
            _coordinates = coordinates;

            _sprites = new Dictionary<CityLayout.BuildingType, Sprite>()
            {
                {CityLayout.BuildingType.Normal, normalSprite},
                {CityLayout.BuildingType.Water, waterSprite},
                {CityLayout.BuildingType.Park, parkSprite}
            };

            _currentState = (CityLayout.BuildingType)state;
            UpdateAppearance();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _gridCreationTool.BuildingClicked(_coordinates);
        }

        public void SetWaterState(bool isWater)
        {
            _currentState = isWater ? CityLayout.BuildingType.Water : CityLayout.BuildingType.Normal;
            UpdateAppearance();
        }

        public void SetParkState(bool isPark)
        {
            _currentState = isPark ? CityLayout.BuildingType.Park : CityLayout.BuildingType.Normal;
            UpdateAppearance();
        }

        private void UpdateAppearance()
        {
            image.sprite = _sprites[_currentState];
        }
    }
}

#endif