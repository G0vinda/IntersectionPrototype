using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GridCreationTool
{
    public class GridCreationIntersection : MonoBehaviour
    {
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite blockedSprite;
        
        public State currentState;
        
        private Image _image;
        private int _numberOfConnectedOpenStreets;
        
        public void Initialize(Vector2Int coordinates, int maxX, int maxY, int state)
        {
            _image = GetComponent<Image>();
            _numberOfConnectedOpenStreets = 4;
            if (coordinates.x == 0 || coordinates.x == maxX)
                _numberOfConnectedOpenStreets--;

            if (coordinates.y == 0 || coordinates.y == maxY)
                _numberOfConnectedOpenStreets--;

            if ((State)state == State.Normal)
            {
                SetStateToNormal();
            }
            else
            {
                SetStateToBlocked();
            }
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
            currentState = State.Blocked;
            UpdateAppearance();
        }

        private void SetStateToNormal()
        {
            currentState = State.Normal;
            UpdateAppearance();
        }

        private void UpdateAppearance()
        {
            _image.sprite = currentState == State.Normal ? normalSprite : blockedSprite;
        }
        
        public enum State
        {
            Normal,
            Blocked
        }
    }
}