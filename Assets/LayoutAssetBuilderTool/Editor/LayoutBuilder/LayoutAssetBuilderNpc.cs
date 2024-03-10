using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
{
    public class LayoutAssetBuilderNpc : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Sprite upSprite;
        [SerializeField] private Sprite rightSprite;
        [SerializeField] private Sprite downSprite;
        [SerializeField] private Sprite leftSprite;

        private List<GameObject> _wayPoints = new ();
        
        public CityLayout.Direction NpcDirection
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
        private Dictionary<CityLayout.Direction, Sprite> _sprites;
        private CityLayout.Direction _npcDirection;

        public void Initialize(GridCreationTool gridCreationTool, Vector2Int coordinates, CityLayout.Direction direction)
        {
            _gridCreationTool = gridCreationTool;
            _image = GetComponentInChildren<Image>();
            _sprites = new Dictionary<CityLayout.Direction, Sprite>()
            {
                { CityLayout.Direction.Up, upSprite },
                { CityLayout.Direction.Right, rightSprite },
                { CityLayout.Direction.Down, downSprite },
                { CityLayout.Direction.Left, leftSprite }
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
    }
}

#endif