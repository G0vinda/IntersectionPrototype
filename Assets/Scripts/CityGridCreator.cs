using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using Random = UnityEngine.Random;

public class CityGridCreator : MonoBehaviour
{
    [SerializeField] private int gridXSize;
    [SerializeField] private int gridYSize;
    [SerializeField] private float cityBlockDistance;

    [SerializeField] private Color mostPrivilegedColor;
    [SerializeField] private Color secondPrivilegedColor;
    
    [Header("EnvironmentPrefabs")]
    [SerializeField] private GameObject cityBlockPrefab;
    [SerializeField] private TunnelBlock tunnelBlockPrefab;
    [SerializeField] private GameObject intersectionPrefab;
    [SerializeField] private GameObject streetPrefab;

    [SerializeField] private CharacterAppearance npcPrefab;

    private Dictionary<Vector2Int, GameObject> _cityGrid = new ();
    private float _halfCityBlockDistance;
    private int _currentMinYLevel = 0;
    private int _currentMaxYLevel;
    private int _lastRowType = -1;

    public void CreateNewCityGrid()
    {
        _currentMaxYLevel = 0;
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        
        _halfCityBlockDistance = cityBlockDistance / 2.0f;
        
        GenerateRow(0, false);
        for (var y = 1; y < gridYSize; y++)
        {
            GenerateNextRowInFront();
        }
    }

    public bool TryGetIntersectionPosition(Vector2Int coordinates, out Vector3 intersectionPosition)
    {
        intersectionPosition = Vector3.negativeInfinity;
        if (!_cityGrid.ContainsKey(coordinates))
            return false;
        
        intersectionPosition = _cityGrid[coordinates].transform.position;
        return true;
    }

    public (int, int) GetCurrentXBounds()
    {
        return (0, gridXSize - 1);
    }

    public (int, int) GetCurrentYBounds()
    {
        return (_currentMaxYLevel - gridYSize, _currentMaxYLevel);
    }

    public void GenerateNextRowInFront()
    {
        GenerateRow(++_currentMaxYLevel);
    }

    public void GenerateNextRowInBack()
    {
        GenerateRow(--_currentMinYLevel);
    }

    public void GenerateRow(int yLevel, bool withTunnels = true)
    {
        List<int> tunnelBlockPositions = new List<int>();
        if (withTunnels)
        {
            int rowType;
            do
            {
                rowType = Random.Range(0, 4);
            } while (rowType == _lastRowType);
            tunnelBlockPositions = rowType == 3 ? new List<int> { 0, 2 } : new List<int> { rowType };
        }
        
        for (var x = 0; x < gridXSize; x++)
        {
            var intersectionPosition = new Vector3(x * cityBlockDistance, 0 , yLevel * cityBlockDistance);
            CreateIntersection(intersectionPosition, x, yLevel);
                
            if (x == 0)
            {
                var firstCityBlockPosition = intersectionPosition + new Vector3(-_halfCityBlockDistance, 0, -_halfCityBlockDistance);
                CreateCityBlock(firstCityBlockPosition, false);
            }

            if (withTunnels && tunnelBlockPositions.Contains(x))
            {
                var tunnelBlockPosition =
                    intersectionPosition + new Vector3(cityBlockDistance, 0, -_halfCityBlockDistance);
                CreateTunnelBlock(tunnelBlockPosition, x, yLevel);
                x++;
            }
            else
            {
                var cityBlockPosition =
                    intersectionPosition + new Vector3(_halfCityBlockDistance, 0, -_halfCityBlockDistance);
                CreateCityBlock(cityBlockPosition);
            }
        }

        if (yLevel != 0)
            GenerateNpc(yLevel);
    }

    private void CreateIntersection(Vector3 intersectionPosition, int x, int y)
    {
        _cityGrid[new Vector2Int(x, y)] = Instantiate(intersectionPrefab, intersectionPosition, Quaternion.identity, transform);
    }

    private void CreateCityBlock(Vector3 cityBlockPosition, bool withLeftWall = true)
    {
        Instantiate(cityBlockPrefab, cityBlockPosition, Quaternion.identity, transform);
        Instantiate(streetPrefab, cityBlockPosition + new Vector3(0, 0, _halfCityBlockDistance), Quaternion.identity, transform);
        if (withLeftWall)
        {
            Instantiate(streetPrefab, cityBlockPosition + new Vector3(-_halfCityBlockDistance, 0, 0), Quaternion.Euler(0, -90, 0),
                transform);
        }
    }

    private void CreateTunnelBlock(Vector3 tunnelBlockPosition, int x, int y)
    {
        var tunnelBlock = Instantiate(tunnelBlockPrefab, tunnelBlockPosition, Quaternion.identity, transform);
        var topLeftStreetPosition =
            tunnelBlockPosition + new Vector3(-_halfCityBlockDistance, 0, _halfCityBlockDistance);
        var topRightStreetPosition =
            tunnelBlockPosition + new Vector3(_halfCityBlockDistance, 0, _halfCityBlockDistance);
        var leftStreetPosition = tunnelBlockPosition + new Vector3(-cityBlockDistance, 0, 0);
        Instantiate(streetPrefab, topLeftStreetPosition, Quaternion.identity, transform);
        Instantiate(streetPrefab, topRightStreetPosition, Quaternion.identity, transform);
        Instantiate(streetPrefab, leftStreetPosition, Quaternion.Euler(0, -90, 0), transform);
        CreateIntersection(tunnelBlockPosition + new Vector3(0, 0, _halfCityBlockDistance), x + 1, y);

        tunnelBlock.SetPrimaryStripeColor(mostPrivilegedColor, CharacterAttributes.CharColor.Blue);
        if (Random.Range(0, 2) > 0)
        {
            tunnelBlock.SetSecondaryStripeColor(mostPrivilegedColor, CharacterAttributes.CharColor.Blue);
        }
        else
        {
            tunnelBlock.SetSecondaryStripeColor(secondPrivilegedColor, CharacterAttributes.CharColor.Green);
        }
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
        newNpcMovement.Initialize(newNpcCoordinates, this, (CharacterAttributes.CharShape)shapeIndex);
    }

    public void PrepareRowsAhead(int newYPosition)
    {
        var rowDiff = newYPosition + gridYSize - _currentMaxYLevel;
        if(rowDiff <= 0)
            return;
        
        for (var i = 0; i < rowDiff; i++)
        {
            GenerateNextRowInFront();
        }
    }

    public void PrepareRowsInBack(int newYPosition)
    {
        var rowDiff = _currentMinYLevel - newYPosition;
        if (rowDiff <= 0)
            return;

        for (var i = 0; i < rowDiff; i++)
        {
            GenerateNextRowInBack();
        }
    }
}
