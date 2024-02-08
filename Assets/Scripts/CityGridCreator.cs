using System;
using System.Collections.Generic;
using System.Linq;
using Character;
using Helpers;
using Newtonsoft.Json;
using UnityEngine;
using GridCreationTool;
using Random = UnityEngine.Random;

public class CityGridCreator : MonoBehaviour
{
    [SerializeField] private int gridXSize;
    [SerializeField] private int gridYSize;
    [SerializeField] private float cityBlockDistance;
    [SerializeField] private TextAsset layoutBlockDataFile;

    [SerializeField] private Color mostPrivilegedColor;
    [SerializeField] private Color secondPrivilegedColor;
    
    [Header("EnvironmentPrefabs")]
    [SerializeField] private GameObject cityBlockPrefab;
    [SerializeField] private GameObject verticalBetweenBlockPrefab;
    [SerializeField] private GameObject horizontalBetweenBlockPrefab;
    [SerializeField] private TunnelBlock verticalTunnelBlockPrefab;
    [SerializeField] private TunnelBlock horizontalTunnelBlockPrefab;
    [SerializeField] private GameObject intersectionPrefab;
    [SerializeField] private GameObject streetPrefab;
    [SerializeField] private GameObject sideWallPrefab;

    [SerializeField] private CharacterAppearance npcPrefab;

    private Dictionary<Vector2Int, GameObject> _cityGrid = new ();
    private float _halfCityBlockDistance;
    private int _currentMinYLevel;
    private int _currentMaxYLevel;
    private readonly float _sideWallOffset = 1.25f;
    private List<GridCreationTool.GridCreationTool.LayoutBlockData> _layouts;

    private void Awake()
    {
        var layoutsString = layoutBlockDataFile.ToString();
        _layouts = JsonConvert.DeserializeObject<List<GridCreationTool.GridCreationTool.LayoutBlockData>>(layoutsString);
    }

    public void CreateNewCityGrid()
    {
        _currentMaxYLevel = 0;
        _halfCityBlockDistance = cityBlockDistance * 0.5f;

        BuildStartLayoutBlock();
        for (var i = 0; i < 3; i++)
        {
            BuildNewLayoutBlock(_layouts.ElementAt(i));
        }
    }

    public void DeleteCurrentCityGrid()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    // intersectionPosition will always be set, even if the position is outside of the cityGrid. In this case false is returned. 
    public bool TryGetIntersectionPosition(Vector2Int coordinates, out Vector3 intersectionPosition)
    {
        if (coordinates.y > _currentMaxYLevel - gridYSize)
        {
            var rowDiff = coordinates.y + gridYSize - _currentMaxYLevel;

            var layoutBlocksToCreate = Mathf.CeilToInt(rowDiff / 3f);
            for (var i = 0; i < layoutBlocksToCreate; i++)
            {
                BuildNewLayoutBlock();
            }
        }else if (coordinates.y < _currentMinYLevel)
        {
            var rowDiff = _currentMinYLevel - coordinates.y;
            
            var layoutBlocksToCreate = Mathf.CeilToInt(rowDiff / 3f);
            for (var i = 0; i < layoutBlocksToCreate; i++)
            {
                BuildNewLayoutBlock(null, false);
            }
        }

        if (coordinates.x < 0 || coordinates.x >= gridXSize)
        {
            // used for bumping into the side wall
            var virtualPositionOutsideGrid = coordinates.x < 0
                ? _cityGrid[new Vector2Int(0, coordinates.y)].transform.position + new Vector3(-cityBlockDistance, 0, 0)
                : _cityGrid[new Vector2Int(gridXSize - 1, coordinates.y)].transform.position + new Vector3(cityBlockDistance, 0, 0);

            intersectionPosition = virtualPositionOutsideGrid;
            return false;
        }

        intersectionPosition = _cityGrid[coordinates].transform.position;
        return true;
    }

    private void BuildStartLayoutBlock()
    {
        _currentMinYLevel = 1; // This is needed to create the start block properly
        BuildNewLayoutBlock(null, false);
    }

    private void BuildNewLayoutBlock(GridCreationTool.GridCreationTool.LayoutBlockData data = null, bool inFront = true)
    {
        if (data == null) // Change this in the future
        {
            data = _layouts.ElementAt(Random.Range(0, 3));
        }
            
        var layout = data.State;
        var layoutHeight = 6;
        var layoutWidth = 9;
        var bottomLeftRowPosition = inFront
            ? new Vector3(0, 0, (_currentMaxYLevel + 1) * cityBlockDistance)
            : new Vector3(0, 0, (_currentMinYLevel - 3) * cityBlockDistance);
            
        Instantiate(sideWallPrefab, bottomLeftRowPosition + new Vector3(-_halfCityBlockDistance, 0, 0),
            Quaternion.identity, transform);
        for (var x = 0; x < layoutWidth; x++)
        {
            if (x % 2 == 0)
            {
                var yIntersectionCoordinate = inFront ? _currentMaxYLevel + 1 : _currentMinYLevel - 3;
                BuildLayoutIntersectionColumn(layout, layoutHeight, x, yIntersectionCoordinate, bottomLeftRowPosition);
            }
            else
            {
                BuildLayoutBuildingColumn(layout, layoutHeight, x, bottomLeftRowPosition);
            }
                
            bottomLeftRowPosition += new Vector3(_halfCityBlockDistance, 0, 0);
        }
        Instantiate(sideWallPrefab, bottomLeftRowPosition, Quaternion.identity, transform);

        if (inFront)
        {
            _currentMaxYLevel += 3;
        }
        else
        {
            _currentMinYLevel -= 3;
        }
    }

    private void BuildLayoutIntersectionColumn(int[,] columnData, int layoutHeight, int xCoordinate, int startYCoordinate, Vector3 position)
    {
        for (var y = layoutHeight - 1; y >= 0; y--)
        {
            if (y % 2 == 1) // Create street
            {
                switch ((GridCreationStreet.State)columnData[xCoordinate, y])
                {
                    case GridCreationStreet.State.Normal:
                        Instantiate(streetPrefab, position, Quaternion.Euler(0, 90, 0), transform);
                        break;
                    case GridCreationStreet.State.Blocked:
                        Instantiate(verticalBetweenBlockPrefab, position, Quaternion.identity, transform);
                        break;
                    case GridCreationStreet.State.Tunnel:
                        var tunnel = Instantiate(verticalTunnelBlockPrefab, position, Quaternion.identity,
                            transform);
                        tunnel.SetPrimaryStripeColor(mostPrivilegedColor, CharacterAttributes.CharColor.Blue);
                        if (Random.Range(0, 2) == 0)
                        {
                            tunnel.SetSecondaryStripeColor(mostPrivilegedColor, CharacterAttributes.CharColor.Blue);
                        }
                        else
                        {
                            tunnel.SetSecondaryStripeColor(secondPrivilegedColor, CharacterAttributes.CharColor.Red);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else // Create intersection
            {
                var intersectionX = Mathf.FloorToInt(xCoordinate / 2f);
                var intersectionY = startYCoordinate + 2 - Mathf.FloorToInt(y / 2f);
                InstantiateIntersection(new Vector2Int(intersectionX, intersectionY), position);
            }

            position += new Vector3(0, 0, _halfCityBlockDistance);
        }
    }

    private void BuildLayoutBuildingColumn(int[,] columnData, int layoutHeight, int xCoordinate, Vector3 position)
    {
        for (var y = layoutHeight - 1; y >= 0; y--)
        {
            if (y % 2 == 1) // Create Building
            {
                Instantiate(cityBlockPrefab, position, Quaternion.identity, transform);
            }
            else // Create street
            {
                switch ((GridCreationStreet.State)columnData[xCoordinate, y])
                {
                    case GridCreationStreet.State.Normal:
                        Instantiate(streetPrefab, position, Quaternion.identity, transform);
                        break;
                    case GridCreationStreet.State.Blocked:
                        Instantiate(horizontalBetweenBlockPrefab, position, Quaternion.identity, transform);
                        break;
                    case GridCreationStreet.State.Tunnel:
                        var tunnel = Instantiate(horizontalTunnelBlockPrefab, position, Quaternion.identity,
                            transform);
                        tunnel.SetPrimaryStripeColor(mostPrivilegedColor, CharacterAttributes.CharColor.Blue);
                        if (Random.Range(0, 2) == 0)
                        {
                            tunnel.SetSecondaryStripeColor(mostPrivilegedColor, CharacterAttributes.CharColor.Blue);
                        }
                        else
                        {
                            tunnel.SetSecondaryStripeColor(secondPrivilegedColor, CharacterAttributes.CharColor.Red);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            position += new Vector3(0, 0, _halfCityBlockDistance);
        }
    }

    private void InstantiateIntersection(Vector2Int coordinates, Vector3 worldPosition)
    {
        var newIntersection = Instantiate(intersectionPrefab, worldPosition, Quaternion.identity, transform);
        _cityGrid[coordinates] = newIntersection;
    }
    
    private void GenerateNpc(int yCoordinate)
    {
        var newNpcCoordinates = new Vector2Int(Random.Range(0, gridXSize), yCoordinate);
        TryGetIntersectionPosition(newNpcCoordinates, out var newNpcPosition);
        
        var newNpcAppearance = Instantiate(npcPrefab, newNpcPosition + new Vector3(0, 3f, 0), Quaternion.identity, transform);
        newNpcAppearance.Initialize();
        var shapeIndex = Random.Range(0, newNpcAppearance.GetShapesLength());
        var colorIndex = Random.Range(0, newNpcAppearance.GetColorLength());
        newNpcAppearance.SetAppearance(shapeIndex, colorIndex);

        var newNpcMovement = newNpcAppearance.GetComponent<NpcMovement>();
        //newNpcMovement.Initialize(newNpcCoordinates, this, (CharacterAttributes.CharShape)shapeIndex);
    }
}