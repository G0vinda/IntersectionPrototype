using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
{
    public class GridCreationBetweenPart : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite blockedSprite;
        [SerializeField] private Sprite tunnelSprite;
        [SerializeField] private Sprite waterSprite;
        [SerializeField] private Sprite parkSprite;
        
        [SerializeField] private Image image;
        
        private GridCreationTool _gridCreationTool;
        private Dictionary<CityLayout.BetweenPartType, Sprite> _sprites;
        private Vector2Int _coordinates;
        private CityLayout.BetweenPartType _currentState;

        public void Initialize(GridCreationTool gridCreationTool, Vector2Int coordinates, int state)
        {
            _gridCreationTool = gridCreationTool;
            _coordinates = coordinates;
            
            _sprites = new Dictionary<CityLayout.BetweenPartType, Sprite>()
            {
                { CityLayout.BetweenPartType.Normal, normalSprite },
                { CityLayout.BetweenPartType.Blocked, blockedSprite },
                { CityLayout.BetweenPartType.Tunnel, tunnelSprite },
                { CityLayout.BetweenPartType.Water, waterSprite },
                { CityLayout.BetweenPartType.Park, parkSprite }
            };

            _currentState = (CityLayout.BetweenPartType)state;
            UpdateAppearance();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _gridCreationTool.StreetClicked(_coordinates);
        }

        public CityLayout.BetweenPartType ChangeState()
        {
            var nextState = (int)_currentState > 2 ? 0 : (CityLayout.BetweenPartType)(((int)_currentState + 1) % 3);
            _currentState = nextState;
            UpdateAppearance();

            return _currentState;
        }

        public void SetWaterState(bool isWater)
        {
            _currentState = isWater ? CityLayout.BetweenPartType.Water : CityLayout.BetweenPartType.Normal;
            UpdateAppearance();
        }

        public void SetParkState(bool isPark)
        {
            _currentState = isPark ? CityLayout.BetweenPartType.Park : CityLayout.BetweenPartType.Normal;
            UpdateAppearance();
        }

        private void UpdateAppearance()
        {
            image.sprite = _sprites[_currentState];
        }
    }
}

#endif