using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace GridCreationTool
{
    public class GridCreationTool : MonoBehaviour
    {
        [SerializeField] private GridCreationStreet streetPrefabHorizontal;
        [SerializeField] private GridCreationStreet streetPrefabVertical;
        [SerializeField] private GridCreationIntersection intersectionPrefab;
        [SerializeField] private GameObject buildingBlockPrefab;
        [SerializeField] private RectTransform gridTopLeftCorner;
        [SerializeField] private float buildingBlockSize;
        [SerializeField] private TextAsset layoutBlockDataFile;
        [SerializeField] private LayoutFieldContainer layoutFieldContainer;
        [SerializeField] private GameObject menuBackground;
        [SerializeField] private GameObject editBackground;

        private const int MaxGridX = 9;
        private const int MaxGridY = 6;

        private GameObject[ , ] _grid = new GameObject[MaxGridX, MaxGridY];
        private List<LayoutBlockData> _layouts;
        private LayoutBlockData _gridState;
        private int _currentEditIndex = -1;

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
            var leftPosition = (Vector2)gridTopLeftCorner.localPosition;
            for (var y = 0; y < MaxGridY; y++)
            {
                var newLeftPosition = leftPosition -
                    new Vector2(0, y / 2 * 1.5f * buildingBlockSize + y % 2 * 0.5f * buildingBlockSize);

                if (y % 2 == 0)
                {
                    BuildIntersectionRow(newLeftPosition, y);
                }
                else
                {
                    BuildBuildingRow(newLeftPosition, y);
                }
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
            
            newIntersection.Initialize(coordinates, MaxGridX - 1, MaxGridY -1, _gridState.State[coordinates.x, coordinates.y]);
        }

        private void BuildStreet(Vector2 position, bool horizontal, Vector2Int coordinates)
        {
            var newStreet = Instantiate(horizontal ? streetPrefabHorizontal : streetPrefabVertical, editBackground.transform);
            newStreet.transform.localPosition = position;
            _grid[coordinates.x, coordinates.y] = newStreet.gameObject;
            
            newStreet.Initialize(this, coordinates, _gridState.State[coordinates.x, coordinates.y]);
        }

        private void BuildBuildingBlock(Vector2 position, Vector2Int coordinates)
        {
            var newBuilding = Instantiate(buildingBlockPrefab, editBackground.transform);
            newBuilding.transform.localPosition = position;
            _grid[coordinates.x, coordinates.y] = newBuilding;
        }

        public void StreetClicked(Vector2Int streetCoordinates)
        {
            var clickedObject = _grid[streetCoordinates.x, streetCoordinates.y];
            if(!clickedObject.TryGetComponent(out GridCreationStreet street))
                return;
            
            var newStreetState = street.ChangeState();
            _gridState.State[streetCoordinates.x, streetCoordinates.y] = (int)newStreetState;
            
            bool streetJustOpened; 
            switch (newStreetState)
            {
                case GridCreationStreet.State.Blocked:
                    streetJustOpened = false;
                    break;
                case GridCreationStreet.State.Tunnel:
                    streetJustOpened = true;
                    break;
                default:
                    return;
            }
            
            
            if (street.horizontal)
            {
                var leftIntersection = _grid[streetCoordinates.x - 1, streetCoordinates.y].GetComponent<GridCreationIntersection>();
                var rightIntersection = _grid[streetCoordinates.x + 1, streetCoordinates.y].GetComponent<GridCreationIntersection>();

                if (streetJustOpened)
                {
                    leftIntersection.AddOpenStreet();
                    rightIntersection.AddOpenStreet();
                }
                else
                {
                    leftIntersection.RemoveOpenStreet();
                    rightIntersection.RemoveOpenStreet();   
                }

                _gridState.State[streetCoordinates.x - 1, streetCoordinates.y] = (int)leftIntersection.currentState;
                _gridState.State[streetCoordinates.x + 1, streetCoordinates.y] = (int)rightIntersection.currentState;
            }
            else
            {
                var upperIntersection = _grid[streetCoordinates.x, streetCoordinates.y - 1]
                    .GetComponent<GridCreationIntersection>();

                if (streetJustOpened)
                {
                    upperIntersection.AddOpenStreet();
                }
                else
                {
                    upperIntersection.RemoveOpenStreet();
                }
                
                _gridState.State[streetCoordinates.x, streetCoordinates.y - 1] = (int)upperIntersection.currentState;

                if (streetCoordinates.y < MaxGridY - 1)
                {
                    var lowerIntersection = _grid[streetCoordinates.x, streetCoordinates.y + 1]
                        .GetComponent<GridCreationIntersection>();

                    if (streetJustOpened)
                    {
                        lowerIntersection.AddOpenStreet();
                    }
                    else
                    {
                        lowerIntersection.RemoveOpenStreet();
                    }   
                    
                    _gridState.State[streetCoordinates.x, streetCoordinates.y - 1] = (int)lowerIntersection.currentState;
                }
            }
        }
        
        public void WriteTextToLayoutBlockFile()
        {
            _gridState.WriteToTextAsset(layoutBlockDataFile);
        }
        
        [Serializable]
        public class LayoutBlockData
        {
            public int[,] State;

            public LayoutBlockData(int maxX, int maxY)
            {
                State = new int[maxX, maxY];
            }

            public void WriteToTextAsset(TextAsset asset)
            {
                
            }
        }
    }
}
