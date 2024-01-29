using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GridCreationTool
{
    public class GridCreationStreet : MonoBehaviour, IPointerClickHandler
    {
        public bool horizontal;
        
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite blockedSprite;
        [SerializeField] private Sprite tunnelSprite;
        [SerializeField] private Image image;
        
        private GridCreationTool _gridCreationTool;
        private Dictionary<State, Sprite> _sprites;
        private Vector2Int _coordinates;
        private State _currentState;

        public void Initialize(GridCreationTool gridCreationTool, Vector2Int coordinates)
        {
            _gridCreationTool = gridCreationTool;
            _coordinates = coordinates;
            
            _sprites = new Dictionary<State, Sprite>()
            {
                { State.Normal, normalSprite },
                { State.Blocked, blockedSprite },
                { State.Tunnel, tunnelSprite }
            };

            _currentState = State.Normal;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _gridCreationTool.StreetClicked(_coordinates);
        }

        public State ChangeState()
        {
            var nextState = (State)(((int)_currentState + 1) % 3);
            _currentState = nextState;
            UpdateAppearance();

            return _currentState;
        }

        private void UpdateAppearance()
        {
            image.sprite = _sprites[_currentState];
        }
        
        public enum State
        {
            Normal,
            Blocked,
            Tunnel
        }
    }
}