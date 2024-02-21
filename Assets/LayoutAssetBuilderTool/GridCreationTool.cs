using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using GridCreationTool;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace LayoutAssetBuilderTool
{
    public class GridCreationTool : MonoBehaviour
    {
        [SerializeField] private RectTransform gridTopLeftCorner;
        [SerializeField] private float buildingBlockSize;
        [SerializeField] private TextAsset layoutBlockDataFile;
        [SerializeField] private LayoutFieldContainer layoutFieldContainer;
        [SerializeField] private GameObject menuBackground;
        [SerializeField] private GameObject editBackground;
        [Header("Prefabs")]
        [SerializeField] private GridCreationStreet streetPrefabHorizontal;
        [SerializeField] private GridCreationStreet streetPrefabVertical;
        [SerializeField] private GridCreationIntersection intersectionPrefab;
        [SerializeField] private GridCreationBuilding buildingPrefab;
        [SerializeField] private LayoutAssetBuilderNpc npcPrefab;
        [SerializeField] private GameObject horizontalWayPointPrefab;
        [SerializeField] private GameObject verticalWayPointPrefab;
        
        public Tool CurrentTool { get; set; }

        private const int MaxGridX = 9;
        private const int MaxGridY = 6;

        private GameObject[,] _grid = new GameObject[MaxGridX, MaxGridY];
        private List<LayoutBlockData> _layouts;
        private LayoutBlockData _gridState;
        private int _currentEditIndex = -1;
        private List<LayoutAssetBuilderNpc> _npcsOnGrid = new(); 

        #region OnEnable/OnDisable

        private void OnEnable()
        {
            LayoutUIField.EditButtonPressed += OpenLayoutForEdit;
            LayoutUIField.DeleteButtonPressed += DeleteLayout;
        }

        private void OnDisable()
        {
            LayoutUIField.EditButtonPressed -= OpenLayoutForEdit;
            LayoutUIField.DeleteButtonPressed -= DeleteLayout;
        }

        #endregion

        private void Start()
        {
            var layoutsString = layoutBlockDataFile.ToString();
            _layouts = JsonConvert.DeserializeObject<List<LayoutBlockData>>(layoutsString);
            if (_layouts == null)
            {
                _layouts = new List<LayoutBlockData>();
            }

            layoutFieldContainer.SetupLayouts(_layouts);
        }

        public void OpenNewLayout()
        {
            _currentEditIndex = _layouts.Count;
            _gridState = new LayoutBlockData(MaxGridX, MaxGridY);
            menuBackground.SetActive(false);
            editBackground.SetActive(true);
            BuildGrid();
        }

        private void OpenLayoutForEdit(int index)
        {
            _currentEditIndex = index;
            _gridState = _layouts.ElementAt(index);
            menuBackground.SetActive(false);
            editBackground.SetActive(true);
            BuildGrid();
        }

        private void DeleteLayout(int index)
        {
            _layouts.RemoveAt(index);
            layoutFieldContainer.SetupLayouts(_layouts);

            var writeString = JsonConvert.SerializeObject(_layouts);
            File.WriteAllText(AssetDatabase.GetAssetPath(layoutBlockDataFile), writeString);
            EditorUtility.SetDirty(layoutBlockDataFile);
        }

        public void SaveLayout()
        {
            if (_currentEditIndex >= _layouts.Count)
            {
                _layouts.Add(_gridState);
            }
            else
            {
                _layouts.RemoveAt(_currentEditIndex);
                _layouts.Insert(_currentEditIndex, _gridState);
            }

            var writeString = JsonConvert.SerializeObject(_layouts);
            File.WriteAllText(AssetDatabase.GetAssetPath(layoutBlockDataFile), writeString);
            EditorUtility.SetDirty(layoutBlockDataFile);

            menuBackground.SetActive(true);
            editBackground.SetActive(false);
            layoutFieldContainer.SetupLayouts(_layouts);
        }

        private void BuildGrid()
        {
            _npcsOnGrid.Clear();
            var leftPosition = (Vector2)gridTopLeftCorner.localPosition;
            for (var y = 0; y < MaxGridY; y++)
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
            
            foreach (var npcData in _gridState.NpcState)
            {
                PlaceNpc(npcData);
            }
        }

        private void BuildIntersectionRow(Vector2 leftPosition, int y)
        {
            var currentPosition = leftPosition;
            for (var x = 0; x < MaxGridX; x++)
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
            for (var x = 0; x < MaxGridX; x++)
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
            var newIntersection = Instantiate(intersectionPrefab, editBackground.transform);
            newIntersection.transform.localPosition = position;
            _grid[coordinates.x, coordinates.y] = newIntersection.gameObject;

            newIntersection.Initialize(this, coordinates, _gridState.State[coordinates.x, coordinates.y]);
        }

        private void BuildStreet(Vector2 position, bool horizontal, Vector2Int coordinates)
        {
            var newStreet = Instantiate(horizontal ? streetPrefabHorizontal : streetPrefabVertical,
                editBackground.transform);
            newStreet.transform.localPosition = position;
            _grid[coordinates.x, coordinates.y] = newStreet.gameObject;

            newStreet.Initialize(this, coordinates, _gridState.State[coordinates.x, coordinates.y]);
        }

        private void BuildBuildingBlock(Vector2 position, Vector2Int coordinates)
        {
            var newBuilding = Instantiate(buildingPrefab, editBackground.transform);
            newBuilding.transform.localPosition = position;
            _grid[coordinates.x, coordinates.y] = newBuilding.gameObject;

            newBuilding.Initialize(this, coordinates, _gridState.State[coordinates.x, coordinates.y]);
        }

        public void StreetClicked(Vector2Int streetCoordinates)
        {
            var clickedObject = _grid[streetCoordinates.x, streetCoordinates.y];
            if (!clickedObject.TryGetComponent(out GridCreationStreet street))
                return;

            var currentState = (GridCreationStreet.State)_gridState.State[streetCoordinates.x, streetCoordinates.y];

            if(CurrentTool == Tool.Water)
            {
                street.SetWaterState(currentState != GridCreationStreet.State.Water);
                _gridState.State[streetCoordinates.x, streetCoordinates.y] = currentState == GridCreationStreet.State.Water ? (int)GridCreationStreet.State.Normal : (int)GridCreationStreet.State.Water;
                return;
            }

            if(CurrentTool == Tool.Park)
            {
                street.SetParkState(currentState != GridCreationStreet.State.Park);
                _gridState.State[streetCoordinates.x, streetCoordinates.y] = currentState == GridCreationStreet.State.Park ? (int)GridCreationStreet.State.Normal : (int)GridCreationStreet.State.Park;
                return;
            }

            var newStreetState = street.ChangeState(); // Todo: refactor so state is not handled by visuals
            _gridState.State[streetCoordinates.x, streetCoordinates.y] = (int)newStreetState;
            
            UpdateNpcWayPoints();
        }

        public void IntersectionClicked(Vector2Int intersectionCoordinates)
        {
            if (CurrentTool == Tool.Npc)
            {
                if(_gridState.NpcState.Any(npc => npc.Coordinates == intersectionCoordinates))
                return;

                var newNpcData = new NpcData(intersectionCoordinates, LayoutAssetBuilderNpc.Direction.Up);
                _gridState.NpcState.Add(newNpcData);
                PlaceNpc(newNpcData);
            }
                
            if(CurrentTool == Tool.Water) // Todo: remove Npc if existing
            {
                var clickedObject = _grid[intersectionCoordinates.x, intersectionCoordinates.y];
                if (!clickedObject.TryGetComponent(out GridCreationIntersection intersection))
                    return;

                var currentIntersectionState = (GridCreationIntersection.State)_gridState.State[intersectionCoordinates.x, intersectionCoordinates.y];

                intersection.SetWaterState(currentIntersectionState != GridCreationIntersection.State.Water);
                _gridState.State[intersectionCoordinates.x, intersectionCoordinates.y] = currentIntersectionState == GridCreationIntersection.State.Water ? (int)GridCreationIntersection.State.Normal : (int)GridCreationIntersection.State.Water;
            }
        }

        public void BuildingClicked(Vector2Int buildingCoordinates)
        {
            var clickedObject = _grid[buildingCoordinates.x, buildingCoordinates.y];
            if (!clickedObject.TryGetComponent(out GridCreationBuilding building))
                return;

            var currentBuildingState = (GridCreationBuilding.State)_gridState.State[buildingCoordinates.x, buildingCoordinates.y];

            if(CurrentTool == Tool.Water)
            {
                building.SetWaterState(currentBuildingState != GridCreationBuilding.State.Water);
                _gridState.State[buildingCoordinates.x, buildingCoordinates.y] = currentBuildingState == GridCreationBuilding.State.Water ? (int)GridCreationBuilding.State.Normal : (int)GridCreationBuilding.State.Water;
                return;
            }

            if(CurrentTool == Tool.Park)
            {
                building.SetParkState(currentBuildingState != GridCreationBuilding.State.Park);
                _gridState.State[buildingCoordinates.x, buildingCoordinates.y] = currentBuildingState == GridCreationBuilding.State.Park ? (int)GridCreationBuilding.State.Normal : (int)GridCreationBuilding.State.Park;
                return;
            }
        }
        
        public void NpcClicked(LayoutAssetBuilderNpc npcObject)
        {
            var npc = _gridState.NpcState.Single(npc => npc.Coordinates == npcObject.Coordinates);
            // Turn direction clockwise
            npc.Direction = (LayoutAssetBuilderNpc.Direction)(((int)npc.Direction + 1) % 4);
            npcObject.NpcDirection = npc.Direction;
            npcObject.DeleteWayPoints();
            PlaceNpcWayPoints(npc, npcObject);
        }

        public void NpcRightClicked(LayoutAssetBuilderNpc npcObject)
        {
            var npcToRemove = _gridState.NpcState.Single(npc => npc.Coordinates == npcObject.Coordinates);
            _gridState.NpcState.Remove(npcToRemove);
            Destroy(npcObject.gameObject);
        }

        private void PlaceNpc(NpcData data)
        {
            var coordinates = data.Coordinates;
            var direction = data.Direction;
            
            var npcPosition = _grid[coordinates.x, coordinates.y].transform.position;
            var npc = Instantiate(npcPrefab, npcPosition, Quaternion.identity, editBackground.transform);
            npc.Initialize(this, coordinates, direction);
            _npcsOnGrid.Add(npc);
            PlaceNpcWayPoints(data, npc);
        }

        private void UpdateNpcWayPoints()
        {
            foreach (var npcObject in _npcsOnGrid)
            {
                var coordinates = npcObject.Coordinates;
                var npcData = _gridState.NpcState.Single(npc => npc.Coordinates == coordinates);
                npcObject.DeleteWayPoints();
                PlaceNpcWayPoints(npcData, npcObject);
            }
        }

        private void PlaceNpcWayPoints(NpcData npcData, LayoutAssetBuilderNpc npcObject)
        {
            Vector2Int directionVector;
            GameObject wayPointPrefab;
            switch (npcData.Direction)
            {
                case LayoutAssetBuilderNpc.Direction.Up:
                    directionVector = Vector2Int.down;
                    wayPointPrefab = verticalWayPointPrefab;
                    break;
                case LayoutAssetBuilderNpc.Direction.Right:
                    directionVector = Vector2Int.right;
                    wayPointPrefab = horizontalWayPointPrefab;
                    break;
                case LayoutAssetBuilderNpc.Direction.Down:
                    directionVector = Vector2Int.up;
                    wayPointPrefab = verticalWayPointPrefab;
                    break;
                case LayoutAssetBuilderNpc.Direction.Left:
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
                if (currentCoordinates.x >= MaxGridX || currentCoordinates.y >= MaxGridY || currentCoordinates.x < 0 || currentCoordinates.y < 0)
                {
                    break;
                }
                
                var currentGridObject = _grid[currentCoordinates.x, currentCoordinates.y];
                if (currentGridObject.TryGetComponent<GridCreationIntersection>(out _))
                {
                    var newWaypoint = Instantiate(wayPointPrefab, currentGridObject.transform.position,
                        Quaternion.identity, editBackground.transform);
                    npcObject.AssignWayPoint(newWaypoint);
                    wayPoints.Add(currentCoordinates);
                }
                else // must be street
                {
                    if ((GridCreationStreet.State)_gridState.State[currentCoordinates.x, currentCoordinates.y] !=
                        GridCreationStreet.State.Normal)
                    {
                        break;
                    }
                }
            }

            npcData.Waypoints = wayPoints.ToArray();
        }

        [Serializable]
        public class LayoutBlockData
        {
            public int[,] State;
            public List<NpcData> NpcState;

            public LayoutBlockData(int maxX, int maxY)
            {
                State = new int[maxX, maxY];
                NpcState = new List<NpcData>();
            }
        }
        
        public class NpcData
        {
            public Vector2Int Coordinates;
            public Vector2Int[] Waypoints;
            public LayoutAssetBuilderNpc.Direction Direction;

            public NpcData(Vector2Int coordinates, LayoutAssetBuilderNpc.Direction direction)
            {
                Coordinates = coordinates;
                Waypoints = new Vector2Int[] { };
                Direction = direction;
            }
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