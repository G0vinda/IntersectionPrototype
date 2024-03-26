using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
{
    public class GridCreationTool : MonoBehaviour
    {
        [SerializeField] private RectTransform gridTopLeftCorner;
        [SerializeField] private float buildingBlockSize;
        [SerializeField] private LayoutUIName nameHeader;
        [SerializeField] private LayoutUINameEdit nameHeaderEdit;
        [SerializeField] private DifficultyIcon difficultyIcon;
        [Header("Prefabs")]
        [SerializeField] private GridCreationBetweenPart streetPrefabHorizontal;
        [SerializeField] private GridCreationBetweenPart streetPrefabVertical;
        [SerializeField] private GridCreationIntersection intersectionPrefab;
        [SerializeField] private GridCreationBuilding buildingPrefab;
        [SerializeField] private LayoutAssetBuilderNpc npcPrefab;
        [SerializeField] private GameObject horizontalWayPointPrefab;
        [SerializeField] private GameObject verticalWayPointPrefab;

        public Action<CityLayout.LayoutBlockData> saveButtonClicked;
        public Action cancelButtonClicked;
        
        public Tool CurrentTool { get; set; }

        private GameObject[,] _grid = new GameObject[CityLayout.LayoutBlockData.MaxGridX, CityLayout.LayoutBlockData.MaxGridY];
        private CityLayout.LayoutBlockData _layoutData;
        private List<LayoutAssetBuilderNpc> _npcsOnGrid = new(); 

        #region OnEnable/OnDisable

        void OnEnable()
        {
            nameHeader.GotDoubleClicked += StartNameEdit;
            nameHeaderEdit.NameEditSubmitted += LayoutNameChanged;
            nameHeaderEdit.NameEditCanceled += EndNameEdit;
        }

        void OnDisable()
        {
            nameHeader.GotDoubleClicked -= StartNameEdit;
            nameHeaderEdit.NameEditSubmitted -= LayoutNameChanged;
            nameHeaderEdit.NameEditCanceled -= EndNameEdit;
        }

        #endregion

        public void BuildGrid(CityLayout.LayoutBlockData layoutData)
        {
            nameHeader.text = layoutData.name;
            nameHeader.gameObject.SetActive(true);
            nameHeaderEdit.gameObject.SetActive(false);
            difficultyIcon.SetDifficulty(layoutData.difficulty);

            CurrentTool = Tool.None;
            _layoutData = layoutData;
            _npcsOnGrid.Clear();
            var leftPosition = (Vector2)gridTopLeftCorner.localPosition;

            for (var y = 0; y < CityLayout.LayoutBlockData.MaxGridY; y++)
            {
                var newLeftPosition = leftPosition -
                                      new Vector2(0,
                                          y / 2 * 1.5f * buildingBlockSize + y % 2 * 0.5f * buildingBlockSize);

                if (y % 2 == 0)
                {
                    BuildIntersectionRow(newLeftPosition, y);
                }
                else
                {
                    BuildBuildingRow(newLeftPosition, y);
                }
            }
            
            foreach (var npcData in _layoutData.NpcState)
            {
                PlaceNpc(npcData);
            }
        }

        public void DifficultyChanged(int newDifficulty)
        {
            _layoutData.difficulty = newDifficulty;
            difficultyIcon.SetDifficulty(newDifficulty);
        }

        public void SaveButtonClicked()
        {
            saveButtonClicked?.Invoke(_layoutData);
        }

        public void CancelButtonClicked()
        {
            cancelButtonClicked?.Invoke();
        }

        public void StreetClicked(Vector2Int streetCoordinates)
        {
            var clickedObject = _grid[streetCoordinates.x, streetCoordinates.y];
            if (!clickedObject.TryGetComponent(out GridCreationBetweenPart street))
                return;

            var currentState = (CityLayout.BetweenPartType)_layoutData.State[streetCoordinates.x, streetCoordinates.y];

            if(CurrentTool == Tool.Water)
            {
                street.SetWaterState(currentState != CityLayout.BetweenPartType.Water);
                _layoutData.State[streetCoordinates.x, streetCoordinates.y] = currentState == CityLayout.BetweenPartType.Water ? (int)CityLayout.BetweenPartType.Normal : (int)CityLayout.BetweenPartType.Water;
            }
            else if(CurrentTool == Tool.Park)
            {
                street.SetParkState(currentState != CityLayout.BetweenPartType.Park);
                _layoutData.State[streetCoordinates.x, streetCoordinates.y] = currentState == CityLayout.BetweenPartType.Park ? (int)CityLayout.BetweenPartType.Normal : (int)CityLayout.BetweenPartType.Park;
            }
            else
            {
                var newStreetState = street.ChangeState(); // Todo: refactor so state is not handled by visuals
                _layoutData.State[streetCoordinates.x, streetCoordinates.y] = (int)newStreetState;
            }
            
            UpdateNpcWayPoints();
        }

        public void IntersectionClicked(Vector2Int intersectionCoordinates)
        {
            if (CurrentTool == Tool.Npc)
            {
                if(_layoutData.NpcState.Any(npc => npc.Coordinates == intersectionCoordinates))
                return;

                var newNpcData = new CityLayout.NpcData(intersectionCoordinates, CityLayout.Direction.Up);
                _layoutData.NpcState.Add(newNpcData);
                PlaceNpc(newNpcData);
            }
                
            if(CurrentTool == Tool.Water) // Todo: remove Npc if existing
            {
                var clickedObject = _grid[intersectionCoordinates.x, intersectionCoordinates.y];
                if (!clickedObject.TryGetComponent(out GridCreationIntersection intersection))
                    return;

                var currentIntersectionState = (CityLayout.IntersectionType)_layoutData.State[intersectionCoordinates.x, intersectionCoordinates.y];

                intersection.SetWaterState(currentIntersectionState != CityLayout.IntersectionType.Water);
                _layoutData.State[intersectionCoordinates.x, intersectionCoordinates.y] = currentIntersectionState == CityLayout.IntersectionType.Water ? (int)CityLayout.IntersectionType.Normal : (int)CityLayout.IntersectionType.Water;
            }
        }

        public void BuildingClicked(Vector2Int buildingCoordinates)
        {
            var clickedObject = _grid[buildingCoordinates.x, buildingCoordinates.y];
            if (!clickedObject.TryGetComponent(out GridCreationBuilding building))
                return;

            var currentBuildingState = (CityLayout.BuildingType)_layoutData.State[buildingCoordinates.x, buildingCoordinates.y];

            if(CurrentTool == Tool.Water)
            {
                building.SetWaterState(currentBuildingState != CityLayout.BuildingType.Water);
                _layoutData.State[buildingCoordinates.x, buildingCoordinates.y] = currentBuildingState == CityLayout.BuildingType.Water ? (int)CityLayout.BuildingType.Normal : (int)CityLayout.BuildingType.Water;
                return;
            }

            if(CurrentTool == Tool.Park)
            {
                building.SetParkState(currentBuildingState != CityLayout.BuildingType.Park);
                _layoutData.State[buildingCoordinates.x, buildingCoordinates.y] = currentBuildingState == CityLayout.BuildingType.Park ? (int)CityLayout.BuildingType.Normal : (int)CityLayout.BuildingType.Park;
                return;
            }
        }
        
        public void NpcClicked(LayoutAssetBuilderNpc npcObject)
        {
            var npc = _layoutData.NpcState.Single(npc => npc.Coordinates == npcObject.Coordinates);
            // Turn direction clockwise
            npc.Direction = (CityLayout.Direction)(((int)npc.Direction + 1) % 4);
            npcObject.NpcDirection = npc.Direction;
            npcObject.DeleteWayPoints();
            PlaceNpcWayPoints(npc, npcObject);
        }

        public void NpcRightClicked(LayoutAssetBuilderNpc npcObject)
        {
            var npcToRemove = _layoutData.NpcState.Single(npc => npc.Coordinates == npcObject.Coordinates);
            _layoutData.NpcState.Remove(npcToRemove);
            _npcsOnGrid.Remove(npcObject);
            Destroy(npcObject.gameObject);
        }

        private void BuildIntersectionRow(Vector2 leftPosition, int y)
        {
            var currentPosition = leftPosition;
            for (var x = 0; x < CityLayout.LayoutBlockData.MaxGridX; x++)
            {
                var coordinates = new Vector2Int(x, y);
                if (x % 2 == 0)
                {
                    BuildIntersection(currentPosition, coordinates);
                    currentPosition += new Vector2(buildingBlockSize * 0.5f, 0);
                }
                else
                {
                    BuildStreet(currentPosition, true, coordinates);
                    currentPosition += new Vector2(buildingBlockSize, 0);
                }
            }
        }

        private void BuildBuildingRow(Vector2 leftPosition, int y)
        {
            var currentPosition = leftPosition;
            for (var x = 0; x < CityLayout.LayoutBlockData.MaxGridX; x++)
            {
                var coordinates = new Vector2Int(x, y);
                if (x % 2 == 0)
                {
                    BuildStreet(currentPosition, false, coordinates);
                    currentPosition += new Vector2(buildingBlockSize * 0.5f, 0);
                }
                else
                {
                    BuildBuildingBlock(currentPosition, coordinates);
                    currentPosition += new Vector2(buildingBlockSize, 0);
                }
            }
        }

        private void BuildIntersection(Vector2 position, Vector2Int coordinates)
        {
            var newIntersection = Instantiate(intersectionPrefab, transform);
            newIntersection.transform.localPosition = position;
            _grid[coordinates.x, coordinates.y] = newIntersection.gameObject;

            newIntersection.Initialize(this, coordinates, _layoutData.State[coordinates.x, coordinates.y]);
        }

        private void BuildStreet(Vector2 position, bool horizontal, Vector2Int coordinates)
        {
            var newStreet = Instantiate(horizontal ? streetPrefabHorizontal : streetPrefabVertical, transform);
            newStreet.transform.localPosition = position;
            _grid[coordinates.x, coordinates.y] = newStreet.gameObject;

            newStreet.Initialize(this, coordinates, _layoutData.State[coordinates.x, coordinates.y]);
        }

        private void BuildBuildingBlock(Vector2 position, Vector2Int coordinates)
        {
            var newBuilding = Instantiate(buildingPrefab, transform);
            newBuilding.transform.localPosition = position;
            _grid[coordinates.x, coordinates.y] = newBuilding.gameObject;

            newBuilding.Initialize(this, coordinates, _layoutData.State[coordinates.x, coordinates.y]);
        }

        private void PlaceNpc(CityLayout.NpcData data)
        {
            var coordinates = data.Coordinates;
            var direction = data.Direction;
            
            var npcPosition = _grid[coordinates.x, coordinates.y].transform.position;
            var npc = Instantiate(npcPrefab, npcPosition, Quaternion.identity, transform);
            npc.Initialize(this, coordinates, direction);
            _npcsOnGrid.Add(npc);
            PlaceNpcWayPoints(data, npc);
        }

        private void UpdateNpcWayPoints()
        {
            foreach (var npcObject in _npcsOnGrid)
            {
                var npcData = _layoutData.NpcState.Single(npc => npc.Coordinates == npcObject.Coordinates);
                npcObject.DeleteWayPoints();
                PlaceNpcWayPoints(npcData, npcObject);
            }
        }

        private void PlaceNpcWayPoints(CityLayout.NpcData npcData, LayoutAssetBuilderNpc npcObject)
        {
            Vector2Int directionVector;
            GameObject wayPointPrefab;
            switch (npcData.Direction)
            {
                case CityLayout.Direction.Up:
                    directionVector = Vector2Int.down;
                    wayPointPrefab = verticalWayPointPrefab;
                    break;
                case CityLayout.Direction.Right:
                    directionVector = Vector2Int.right;
                    wayPointPrefab = horizontalWayPointPrefab;
                    break;
                case CityLayout.Direction.Down:
                    directionVector = Vector2Int.up;
                    wayPointPrefab = verticalWayPointPrefab;
                    break;
                case CityLayout.Direction.Left:
                    directionVector = Vector2Int.left;
                    wayPointPrefab = horizontalWayPointPrefab;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var currentCoordinates = npcData.Coordinates;
            var wayPoints = new List<Vector2Int>();

            while (true)
            {
                currentCoordinates += directionVector;
                if (currentCoordinates.x >= CityLayout.LayoutBlockData.MaxGridX || currentCoordinates.y >= CityLayout.LayoutBlockData.MaxGridY || currentCoordinates.x < 0 || currentCoordinates.y < 0)
                {
                    break;
                }
                
                var currentGridObject = _grid[currentCoordinates.x, currentCoordinates.y];
                if (currentGridObject.TryGetComponent<GridCreationIntersection>(out _))
                {
                    var newWaypoint = Instantiate(wayPointPrefab, currentGridObject.transform.position,
                        Quaternion.identity, transform);
                    npcObject.AssignWayPoint(newWaypoint);
                    wayPoints.Add(currentCoordinates);
                }
                else // must be street
                {
                    if ((CityLayout.BetweenPartType)_layoutData.State[currentCoordinates.x, currentCoordinates.y] !=
                        CityLayout.BetweenPartType.Normal)
                    {
                        break;
                    }
                }
            }

            npcData.Waypoints = wayPoints.ToArray();
        }

        private void StartNameEdit()
        {
            nameHeader.gameObject.SetActive(false);
            nameHeaderEdit.gameObject.SetActive(true);
            nameHeaderEdit.StartEdit(_layoutData.name);
        }

        private void LayoutNameChanged(string newName)
        {
            _layoutData.name = newName;
            nameHeader.text = newName;
            EndNameEdit();
        }

        private void EndNameEdit()
        {
            nameHeader.gameObject.SetActive(true);
            nameHeaderEdit.gameObject.SetActive(false);
        }

        public enum Tool
        {
            None,
            Npc,
            Water,
            Park
        }
    }
}

#endif