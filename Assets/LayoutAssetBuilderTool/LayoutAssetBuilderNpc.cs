using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LayoutAssetBuilderTool
{
    public class LayoutAssetBuilderNpc : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Sprite upSprite;
        [SerializeField] private Sprite rightSprite;
        [SerializeField] private Sprite downSprite;
        [SerializeField] private Sprite leftSprite;

        private List<GameObject> _wayPoints = new ();

        public Direction NpcDirection
        {
            get => _npcDirection;
            set
            {
                _npcDirection = value;
                _image.sprite = _sprites[value];
            }
        }
        public Vector2Int Coordinates { get; private set; }

        private GridCreationTool _gridCreationTool;
        private Image _image;
        private Dictionary<Direction, Sprite> _sprites;
        private Direction _npcDirection;

        public void Initialize(GridCreationTool gridCreationTool, Vector2Int coordinates, Direction direction)
        {
            _gridCreationTool = gridCreationTool;
            _image = GetComponentInChildren<Image>();
            _sprites = new Dictionary<Direction, Sprite>()
            {
                { Direction.Up, upSprite },
                { Direction.Right, rightSprite },
                { Direction.Down, downSprite },
                { Direction.Left, leftSprite }
            };
            Coordinates = coordinates;
            NpcDirection = direction;
        }

        public void AssignWayPoint(GameObject wayPoint)
        {
            _wayPoints.Add(wayPoint);
        }

        public void DeleteWayPoints()
        {
            foreach (var wayPoint in _wayPoints)
            {
                Destroy(wayPoint);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _gridCreationTool.NpcRightClicked(this);
            }
            else if(eventData.button == PointerEventData.InputButton.Left)
            {
                _gridCreationTool.NpcClicked(this);   
            }
        }

        private void OnDestroy()
        {
            DeleteWayPoints();
        }

        public enum Direction
        {
            Up,
            Right,
            Down,
            Left
        }
    }
}
