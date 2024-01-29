using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GridCreationTool
{
    public class GridCreationIntersection : MonoBehaviour
    {
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite blockedSprite;

        private Image _image;
        private State _currentState;
        private int _numberOfConnectedOpenStreets;
        
        public void Initialize(Vector2Int coordinates, int maxX, int maxY)
        {
            _image = GetComponent<Image>();
            SetStateToNormal();
            _numberOfConnectedOpenStreets = 4;
            if (coordinates.x == 0 || coordinates.x == maxX)
                _numberOfConnectedOpenStreets--;

            if (coordinates.y == 0 || coordinates.y == maxY)
                _numberOfConnectedOpenStreets--;
        }

        public void AddOpenStreet()
        {
            if(_numberOfConnectedOpenStreets == 0)
                SetStateToNormal();
            
            _numberOfConnectedOpenStreets++;
        }

        public void RemoveOpenStreet()
        {
            _numberOfConnectedOpenStreets--;
            
            if(_numberOfConnectedOpenStreets == 0)
                SetStateToBlocked();
        }

        private void SetStateToBlocked()
        {
            _currentState = State.Blocked;
            UpdateAppearance();
        }

        private void SetStateToNormal()
        {
            _currentState = State.Normal;
            UpdateAppearance();
        }

        private void UpdateAppearance()
        {
            _image.sprite = _currentState == State.Normal ? normalSprite : blockedSprite;
        }
        
        public enum State
        {
            Normal,
            Blocked
        }
    }
}