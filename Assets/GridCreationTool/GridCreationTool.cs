using System;
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

        private const int MaxGridX = 9;
        private const int MaxGridY = 6;

        private GameObject[ , ] _grid = new GameObject[MaxGridX, MaxGridY];
        private int[,] _gridState = new int[MaxGridX, MaxGridY];
        
        private void Start()
        {
            BuildGrid();
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
            var newIntersection = Instantiate(intersectionPrefab, transform);
            newIntersection.transform.localPosition = position;
            _grid[coordinates.x, coordinates.y] = newIntersection.gameObject;
            
            newIntersection.Initialize(coordinates, MaxGridX - 1, MaxGridY -1);
        }

        private void BuildStreet(Vector2 position, bool horizontal, Vector2Int coordinates)
        {
            var newStreet = Instantiate(horizontal ? streetPrefabHorizontal : streetPrefabVertical, transform);
            newStreet.transform.localPosition = position;
            _grid[coordinates.x, coordinates.y] = newStreet.gameObject;
            
            newStreet.Initialize(this, coordinates);
        }

        private void BuildBuildingBlock(Vector2 position, Vector2Int coordinates)
        {
            var newBuilding = Instantiate(buildingBlockPrefab, transform);
            newBuilding.transform.localPosition = position;
            _grid[coordinates.x, coordinates.y] = newBuilding;
        }

        public void StreetClicked(Vector2Int streetCoordinates)
        {
            var clickedObject = _grid[streetCoordinates.x, streetCoordinates.y];
            if(!clickedObject.TryGetComponent(out GridCreationStreet street))
                return;
            
            var newStreetState = street.ChangeState();
            
            var streetJustOpened = false; 
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
                }
            }
        }
    }
}
