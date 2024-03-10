using System;
using System.Collections.Generic;
using System.Linq;
using Character;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public class CityGridCreator : MonoBehaviour
{
    [SerializeField] private int gridXSize;
    [SerializeField] private int gridYSize;
    [SerializeField] private float cityBlockDistance;
    [SerializeField] private TextAsset layoutBlockDataFile;

    [SerializeField] private GameObject sideWallPrefab;
    [SerializeField] private CharacterAppearance npcPrefab;

    [Header("BuildingPrefabs")]
    [SerializeField] private Building oneSideBuildingPrefab;
    [SerializeField] private Building twoSideMiddleBuildingPrefab;
    [SerializeField] private Building twoSideCornerBuilingPrefab;
    [SerializeField] private Building threeSideBuildingPrefab;
    [SerializeField] private Building fourSideBuildingPrefab;
    [SerializeField] private BuildingGroup buildingGroupPrefab;

    [Header("BetweenPartPrefabs")]
    [SerializeField] private GameObject streetPrefab;
    [SerializeField] private Tunnel tunnelPrefab;
    [SerializeField] private BetweenPart betweenPartPrefab;
    [SerializeField] private Park betweenPartParkPrefab;

    [Header("IntersectionPrefabs")]
    [SerializeField] private GameObject intersectionPrefab;

    [Header("ParkPrefabs")]
    [SerializeField] private Park oneSideParkPrefab;
    [SerializeField] private Park twoSideMiddleParkPrefab;
    [SerializeField] private Park twoSideCornerParkPrefab;
    [SerializeField] private Park threeSideParkPrefab;
    [SerializeField] private Park fourSideParkPrefab;

    [Header("OrganizerParents")]
    [SerializeField] private Transform streetsParent;
    [SerializeField] private Transform intersectionsParent;
    [SerializeField] private Transform sideWallsParent;
    [SerializeField] private Transform buildingGroupsParent;
    [SerializeField] private Transform parksParent;
    [SerializeField] private Transform npcsParent;

    [Header("Debug")]
    [SerializeField] List<string> debug_SpawnedLayoutNames = new ();

    private Dictionary<Vector2Int, GameObject> _cityObjects = new ();
    private Dictionary<Vector2Int, GameObject> _intersections = new ();
    private Dictionary<int, int[]> _cityRowData = new();
    private float _halfCityBlockDistance;
    private int _currentMinYLevel;
    private int _currentMaxYLevel;
    private List<CityLayout.LayoutBlockData> _layoutTemplates;
    private Dictionary<BuildingLayoutType, Building> _buildingPrefabs;
    private Dictionary<BuildingLayoutType, Park> _parkPrefabs;
    private CharacterAttributes.SpawnRestrictions _npcSpawnRestrictions;
    private bool _spawnNpcs;

    private void Awake()
    {
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(layoutBlockDataFile));
        var layoutsString = layoutBlockDataFile.ToString();
        _layoutTemplates = JsonConvert.DeserializeObject<List<CityLayout.LayoutBlockData>>(layoutsString);
        _buildingPrefabs = new Dictionary<BuildingLayoutType, Building>
        {
            {BuildingLayoutType.OneSide, oneSideBuildingPrefab},
            {BuildingLayoutType.TwoSideMiddle, twoSideMiddleBuildingPrefab},
            {BuildingLayoutType.TwoSideCorner, twoSideCornerBuilingPrefab},
            {BuildingLayoutType.ThreeSide, threeSideBuildingPrefab},
            {BuildingLayoutType.FourSide, fourSideBuildingPrefab},
        };
        _parkPrefabs = new Dictionary<BuildingLayoutType, Park>
        {
            {BuildingLayoutType.OneSide, oneSideParkPrefab},
            {BuildingLayoutType.TwoSideMiddle, twoSideMiddleParkPrefab},
            {BuildingLayoutType.TwoSideCorner, twoSideCornerParkPrefab},
            {BuildingLayoutType.ThreeSide, threeSideParkPrefab},
            {BuildingLayoutType.FourSide, fourSideParkPrefab},
        };
    }

    public void CreateNewCityGrid(CharacterAttributes.SpawnRestrictions npcSpawnRestrictions, bool withNpcs = true)
    {
        _currentMaxYLevel = 0;
        _halfCityBlockDistance = cityBlockDistance * 0.5f;
        _npcSpawnRestrictions = npcSpawnRestrictions;
        _spawnNpcs = withNpcs;

        BuildStartLayoutBlock();
        for (var i = 0; i < 3; i++)
        {
            BuildNewLayoutBlock();
        }
    }

    public void DeleteCurrentCityGrid()
    {
        DeleteChildrenFromTransform(buildingGroupsParent);
        DeleteChildrenFromTransform(streetsParent);
        DeleteChildrenFromTransform(intersectionsParent);
        DeleteChildrenFromTransform(sideWallsParent);
        DeleteChildrenFromTransform(npcsParent);
        DeleteChildrenFromTransform(parksParent);
        _cityObjects.Clear();
        _cityRowData.Clear();
    }

    private void DeleteChildrenFromTransform(Transform parentTransform)
    {
        for (var i = parentTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(parentTransform.GetChild(i).gameObject);
        }
    }

    // intersectionPosition will always be set, even if the position is outside of the cityGrid. In this case false is returned. 
    public bool TryGetIntersectionPosition(Vector2Int intersectionCoordinates, out Vector3 intersectionPosition)
    {
        if (intersectionCoordinates.x < 0 || intersectionCoordinates.x >= gridXSize)
        {
            // used for bumping into the side wall
            var virtualPositionOutsideGrid = intersectionCoordinates.x < 0
                ? _intersections[new Vector2Int(0, intersectionCoordinates.y)].transform.position + new Vector3(-cityBlockDistance, 0, 0)
                : _intersections[new Vector2Int(gridXSize - 1, intersectionCoordinates.y)].transform.position + new Vector3(cityBlockDistance, 0, 0);

            intersectionPosition = virtualPositionOutsideGrid;
            return false;
        }

        if (intersectionCoordinates.y * 2 > _currentMaxYLevel - gridYSize * 2)
        {
            var rowDiff = intersectionCoordinates.y + gridYSize - Mathf.CeilToInt(_currentMaxYLevel * 0.5f);

            var layoutBlocksToCreate = Mathf.CeilToInt(rowDiff / 3f);
            for (var i = 0; i < layoutBlocksToCreate; i++)
            {
                BuildNewLayoutBlock();
            }
        }
        else if (intersectionCoordinates.y * 2 < _currentMinYLevel)
        {
            var rowDiff = Mathf.CeilToInt(_currentMinYLevel * 0.5f) - intersectionCoordinates.y;
            
            var layoutBlocksToCreate = Mathf.CeilToInt(rowDiff / 3f);
            for (var i = 0; i < layoutBlocksToCreate; i++)
            {
                BuildNewLayoutBlock(null, false);
            }
        }

        intersectionPosition = _intersections[intersectionCoordinates].transform.position;
        return true;
    }

    private void BuildStartLayoutBlock()
    {
        _currentMinYLevel = 1; // This is needed to create the start block properly
        BuildNewLayoutBlock(_layoutTemplates.ElementAt(0), false);
    }

    private void BuildNewLayoutBlock(CityLayout.LayoutBlockData data = null, bool inFront = true)
    {
        if (data == null)
        {
            data = _layoutTemplates.ElementAt(Random.Range(0, _layoutTemplates.Count));
        }

        if(inFront)
        {
            debug_SpawnedLayoutNames.Insert(0, data.name);
        }
        else
        {
            debug_SpawnedLayoutNames.Add(data.name);
        }
            
        var layout = data.State;
        var layoutHeight = 6;
        var layoutWidth = 9;
        var leftRowPosition = inFront
            ? new Vector3(0, 0, (_currentMaxYLevel + 1) * _halfCityBlockDistance)
            : new Vector3(0, 0, (_currentMinYLevel - 6) * _halfCityBlockDistance);
            
        Instantiate(sideWallPrefab, leftRowPosition + new Vector3(-_halfCityBlockDistance, 0, 0), Quaternion.identity, sideWallsParent);
        Instantiate(sideWallPrefab, leftRowPosition + new Vector3(_halfCityBlockDistance * layoutWidth, 0, 0), Quaternion.identity, sideWallsParent);

        var rowList = new List<int[]>();
        for (int y = 0; y < layoutHeight; y++)
        {
            var rowData = new int[layoutWidth];
            for (int x = 0; x < layoutWidth; x++)
            {
                rowData[x] = layout[x, layoutHeight - 1 - y];
            }
            var rowY = inFront ? _currentMaxYLevel + 1 + y : _currentMinYLevel - layoutHeight + y;
            _cityRowData.Add(rowY, rowData);
            rowList.Add(rowData);
        }
        
        for (var y = 0; y < layoutHeight; y++)
        { 
            var rowData = rowList.ElementAt(y);

            var rowY = inFront ? _currentMaxYLevel + 1 + y : _currentMinYLevel - layoutHeight + y;
            if (y % 2 == 0)
            {
                BuildBuildingRow(rowData, leftRowPosition, rowY);
            }
            else
            {
                BuildIntersectionRow(rowData, leftRowPosition, rowY);
            }
            leftRowPosition += new Vector3(0, 0, _halfCityBlockDistance);
        }

        if (inFront)
        {
            _currentMaxYLevel += layoutHeight;
        }
        else
        {
            _currentMinYLevel -= layoutHeight;
        }

        if(!_spawnNpcs)
            return;

        var topLeftCoordinates =
            inFront ? new Vector2Int(0, Mathf.RoundToInt(_currentMaxYLevel * 0.5f)) : new Vector2Int(0, Mathf.CeilToInt(_currentMinYLevel * 0.5f + 2));
        foreach (var npcData in data.NpcState)
        {
            GenerateNpc(topLeftCoordinates, npcData);
        }
    }

    private void BuildIntersectionRow(int[] rowData, Vector3 leftPosition, int rowY)
    {
        var placementPosition = leftPosition;
        for (int x = 0; x < rowData.Length; x++)
        {
            if(x % 2 == 0)
            {
                InstantiateIntersection(rowData[x], new Vector2Int(x, rowY), placementPosition);
            }
            else
            {
                InstantiateBetweenPart(rowData[x], new Vector2Int(x, rowY), placementPosition);
            }

            placementPosition += new Vector3(_halfCityBlockDistance, 0, 0);
        }
    }

    private void BuildBuildingRow(int[] rowData, Vector3 leftPosition, int rowY)
    {
        var placementPosition = leftPosition;
        for (int x = 0; x < rowData.Length; x++)
        {
            if(x % 2 == 0)
            {
                InstantiateBetweenPart(rowData[x], new Vector2Int(x, rowY), placementPosition, false);
            }
            else
            {
                InstantiateBuilding(rowData[x], new Vector2Int(x, rowY), placementPosition);
            }

            placementPosition += new Vector3(_halfCityBlockDistance, 0, 0);
        }
    }

    private void InstantiateIntersection(int intersectionType, Vector2Int coordinates, Vector3 worldPosition)
    {
        GameObject newIntersection;
        switch((CityLayout.IntersectionType)intersectionType)
        {
            case CityLayout.IntersectionType.Water:
            case CityLayout.IntersectionType.Normal:
                newIntersection = Instantiate(intersectionPrefab, worldPosition, Quaternion.identity, intersectionsParent);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _cityObjects[coordinates] = newIntersection;
        
        var intersectionX = Mathf.FloorToInt(coordinates.x / 2f);
        var intersectionY = (int)Mathf.Sign(coordinates.y) * Mathf.FloorToInt(Mathf.Abs(coordinates.y / 2f));
        _intersections[new Vector2Int(intersectionX, intersectionY)] = newIntersection;
    }

    private void InstantiateBetweenPart(int streetType, Vector2Int coordinates, Vector3 worldPosition, bool horizontal = true)
    {
        GameObject newBetweenPartObject;
        var rotation = horizontal ? Quaternion.identity : Quaternion.Euler(0, 90, 0);
        switch ((CityLayout.BetweenPartType)streetType)
        {
            case CityLayout.BetweenPartType.Park:
                var newPark = Instantiate(betweenPartParkPrefab, worldPosition, rotation, parksParent);
                newBetweenPartObject = newPark.gameObject;
                break;
            case CityLayout.BetweenPartType.Water:
            case CityLayout.BetweenPartType.Normal:
                newBetweenPartObject = Instantiate(streetPrefab, worldPosition, rotation, streetsParent);
                break;
            case CityLayout.BetweenPartType.Blocked:
                var newBetweenPart = Instantiate(betweenPartPrefab, worldPosition, rotation, transform);
                GetBuildingGroupForBetweenPart(horizontal, coordinates).AssignObject(newBetweenPart);
                newBetweenPartObject = newBetweenPart.gameObject;
                break;
            case CityLayout.BetweenPartType.Tunnel:
                var newTunnel = Instantiate(tunnelPrefab, worldPosition, rotation, transform);
                GetBuildingGroupForBetweenPart(horizontal, coordinates).AssignObject(newTunnel);
                if (Random.Range(0, 2) == 0)
                {
                    newTunnel.SetSecondaryColor(CharacterAttributes.CharColor.Blue);
                }
                else
                {
                    newTunnel.SetSecondaryColor(CharacterAttributes.CharColor.Red);
                }
                newBetweenPartObject = newTunnel.gameObject;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _cityObjects[coordinates] = newBetweenPartObject;
    }

    private void InstantiateBuilding(int buildingType, Vector2Int coordinates, Vector3 worldPosition)
    {
        GameObject newBuildingObject;
        switch ((CityLayout.BuildingType)buildingType)
        {
            case CityLayout.BuildingType.Park:
                var parkNeighborData = GetBuildingNeighborData(coordinates, new[]{CityLayout.BetweenPartType.Park});
                var parkPrefab = _parkPrefabs[parkNeighborData.GetLayoutType()];
                var newPark = Instantiate(parkPrefab, worldPosition, parkNeighborData.GetRotation(), transform);
                newBuildingObject = newPark.gameObject;
                break;
            case CityLayout.BuildingType.Water:
            case CityLayout.BuildingType.Normal:
                var neighborData = GetBuildingNeighborData(coordinates);
                var buildingPrefab = _buildingPrefabs[neighborData.GetLayoutType()];
                var buildingGroup = GetBuildingGroupForBuildling(neighborData, coordinates);
                var newBuilding = Instantiate(buildingPrefab, worldPosition, neighborData.GetRotation(), transform);
                buildingGroup.AssignObject(newBuilding);
                newBuildingObject = newBuilding.gameObject;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _cityObjects[coordinates] = newBuildingObject;
    }
    
    private void GenerateNpc(Vector2Int layoutBlockTopLeftCoordinate, CityLayout.NpcData npcData)
    {
        var newNpcPosition = LayoutCoordinatesToWorldPosition(layoutBlockTopLeftCoordinate, npcData.Coordinates);
        
        var inWorldWayPoints = new List<Vector3>{newNpcPosition};
        foreach (var npcDataWaypoint in npcData.Waypoints)
        {
            inWorldWayPoints.Add(LayoutCoordinatesToWorldPosition(layoutBlockTopLeftCoordinate, npcDataWaypoint));
        }
        
        var newNpcAppearance = Instantiate(npcPrefab, newNpcPosition + new Vector3(0, 3f, 0), Quaternion.identity, npcsParent);
        var npcAttributes = CharacterAttributes.GetRandomAttributes(_npcSpawnRestrictions);
        newNpcAppearance.SetAttributes(npcAttributes);

        var newNpcMovement = newNpcAppearance.GetComponent<NpcMovement>();
        newNpcMovement.Initialize(inWorldWayPoints.ToArray(), npcAttributes.shape);
    }

    private Vector3 LayoutCoordinatesToWorldPosition(Vector2Int layoutBlockTopLeftCoordinate, Vector2Int layoutCoordinates)
    {
        var inWorldX = Mathf.RoundToInt(layoutCoordinates.x * 0.5f);
        var inWorldY = layoutBlockTopLeftCoordinate.y - Mathf.RoundToInt(layoutCoordinates.y * 0.5f);
        return _intersections[new Vector2Int(inWorldX, inWorldY)].transform.position;
    }

    private NeighborData GetBuildingNeighborData(Vector2Int coordinates, CityLayout.BetweenPartType[] types = null)
    {
        var data = new NeighborData(true, true, true, true);
        if(types == null)
            types = new[]{CityLayout.BetweenPartType.Tunnel, CityLayout.BetweenPartType.Blocked};

        var leftType = (CityLayout.BetweenPartType)_cityRowData[coordinates.y][coordinates.x - 1];
        var rightType = (CityLayout.BetweenPartType)_cityRowData[coordinates.y][coordinates.x + 1];
        if(types.Contains(leftType))
            data.leftFree = false;

        if(types.Contains(rightType))
            data.rightFree = false;

        if(_cityRowData.ContainsKey(coordinates.y - 1))
        {
            var bottomType = (CityLayout.BetweenPartType)_cityRowData[coordinates.y - 1][coordinates.x];
            if(types.Contains(bottomType))
            data.bottomFree = false;
        }

        if(_cityRowData.ContainsKey(coordinates.y + 1))
        {
            var topType = (CityLayout.BetweenPartType)_cityRowData[coordinates.y + 1][coordinates.x];
            if(types.Contains(topType))
            data.topFree = false;
        }

        return data;
    }

    private BuildingGroup GetBuildingGroupForBuildling(NeighborData neighborData, Vector2Int coordinates)
    {
        BuildingGroup buildingGroup = null;
        if(!neighborData.bottomFree)
        {
            buildingGroup = _cityObjects[coordinates - new Vector2Int(0, 1)].transform.parent.GetComponent<BuildingGroup>(); // replace with function on object
        }
        if(!neighborData.leftFree)
        {
            var leftBuildingGroup = _cityObjects[coordinates - new Vector2Int(1, 0)].transform.parent.GetComponent<BuildingGroup>(); // Todo: replace with function on object
            if(buildingGroup != null && leftBuildingGroup != buildingGroup)
            {
                leftBuildingGroup.TransferChildrenTo(buildingGroup);
            }
            else
            {
                buildingGroup = leftBuildingGroup;
            }
        }

        if(buildingGroup == null)
            buildingGroup = Instantiate(buildingGroupPrefab, buildingGroupsParent);

        return buildingGroup;
    }

    private BuildingGroup GetBuildingGroupForBetweenPart(bool horizontal, Vector2Int coordinates)
    {
        if(coordinates == new Vector2Int(1, -1))
            Debug.Log("DeleteMe");

        var neighborOffset = horizontal ? new Vector2Int(0, -1) : new Vector2Int(-1, 0);
        var buildingGroup = _cityObjects.ContainsKey(coordinates + neighborOffset) 
            ? _cityObjects[coordinates + neighborOffset].transform.parent.GetComponent<BuildingGroup>()
            : Instantiate(buildingGroupPrefab, buildingGroupsParent);
        
        return buildingGroup;
    }

    private struct NeighborData
    {
        public bool topFree;
        public bool leftFree;
        public bool rightFree;
        public bool bottomFree;
        public int FreeCount {
            get{
                var count = 0;
                if(topFree) count++;
                if(leftFree) count++;
                if(rightFree) count++;
                if(bottomFree) count++;
                return count;
            } 
            set{}        
        }

        public NeighborData(
            bool top,
            bool left,
            bool right,
            bool bottom   
        ){
            topFree = top;
            leftFree = left;
            rightFree = right;
            bottomFree = bottom;
        }

        public BuildingLayoutType GetLayoutType(){
            var freeCount = FreeCount;
            return freeCount switch{
                0 =>  BuildingLayoutType.NoSide,
                1 =>  BuildingLayoutType.OneSide,
                2 =>  AreSidesOpposite() ? BuildingLayoutType.TwoSideMiddle : BuildingLayoutType.TwoSideCorner,
                3 =>  BuildingLayoutType.ThreeSide,
                4 =>  BuildingLayoutType.FourSide,
                _ =>  throw new ArgumentOutOfRangeException()
            };
        }

        public bool AreSidesOpposite()
        {
            if(FreeCount != 2)
                throw new ArgumentOutOfRangeException();

            var sides = new[]{topFree, rightFree, bottomFree, leftFree};
            var freeIndices = new int[2];
            var indexCount = 0;

            for (int i = 0; i < 4; i++)
            {
                if(sides[i]){
                    freeIndices[indexCount] = i;
                    indexCount++;
                    if(indexCount == 2)
                        break;
                }
            }

            return (freeIndices[1] - freeIndices[0]) % 2 == 0;
        }

        public Quaternion GetRotation()
        {
            return GetLayoutType() switch
            {
                BuildingLayoutType.OneSide => GetRotationForOneSide(),
                BuildingLayoutType.TwoSideMiddle => GetRotationForTwoSideMiddle(),
                BuildingLayoutType.TwoSideCorner => GetRotationForTwoSideCorner(),
                BuildingLayoutType.ThreeSide => GetRotationForThreeSide(),
                BuildingLayoutType.FourSide => Quaternion.identity,
                BuildingLayoutType.NoSide => Quaternion.identity,
                _ => throw new NotImplementedException()
            };
        }

        private Quaternion GetRotationForOneSide()
        {
            if(rightFree)
                return Quaternion.identity;
            
            if(topFree)
                return Quaternion.Euler(0, -90, 0);
            
            if(leftFree)
                return Quaternion.Euler(0, -180, 0);
            
            return Quaternion.Euler(0, 90, 0);
        }       

        private Quaternion GetRotationForTwoSideMiddle()
        {
            return leftFree ? Quaternion.identity : Quaternion.Euler(0, -90, 0);
        }

        private Quaternion GetRotationForTwoSideCorner()
        {
            if(topFree)
            {
                if(rightFree)
                    return Quaternion.identity;
                
                return Quaternion.Euler(0, -90, 0);
            }
            
            if(rightFree)
                return Quaternion.Euler(0, 90, 0);
            
            return Quaternion.Euler(0, -180, 0);
        }

        private Quaternion GetRotationForThreeSide()
        {
            if(!leftFree)
                return Quaternion.identity;
            
            if(!bottomFree)
                return Quaternion.Euler(0, -90, 0);

            if(!rightFree)
                return Quaternion.Euler(0, -180, 0);
            
            return Quaternion.Euler(0, 90, 0);
        }
    }

    private enum BuildingLayoutType
    {
        NoSide,
        OneSide,
        TwoSideMiddle,
        TwoSideCorner,
        ThreeSide,
        FourSide
    }
}