using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrafficManager : MonoBehaviour
{
    [SerializeField] private CityGridCreator cityGrid;
    [SerializeField] private GameObject carPrefab;
    
    [Header("Car spawning Values")]
    [Tooltip("How many cars get spawned per second")]
    [SerializeField] private float carRate;
    [Tooltip("The horizontal or vertical offset to the closest intersection")]
    [SerializeField] private float carSpawnOffset;
    [SerializeField] private float carRoadOffset;
    [Tooltip("How high above ground should the car be spawned")]
    [SerializeField] private float carYOffset;

    private WaitForSeconds _spawnWait;
    private bool _previousCarWasHorizontal;
    private Coroutine _trafficRoutine;

    private void Awake()
    {
        _spawnWait = new WaitForSeconds(1f / carRate);
    }

    public void RestartTraffic()
    {
        if (_trafficRoutine != null)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            StopCoroutine(_trafficRoutine);   
        }

        _trafficRoutine = StartCoroutine(CarSpawning());
    }

    private IEnumerator CarSpawning()
    {
        while (true)
        {
            SpawnCar();
            yield return _spawnWait;
        }
    }

    private void SpawnCar()
    {
        Vector3 carSpawnPosition;
        Quaternion carSpawnRotation;

        bool carIsHorizontal;
        do
        {
            carIsHorizontal = Random.Range(0, 2) == 1;
        } while (carIsHorizontal == _previousCarWasHorizontal);
        
        if (carIsHorizontal)
        {
            var carComesFromLeft = Random.Range(0, 2) == 1;
            var (minX, maxX) = cityGrid.GetCurrentXBounds();
            var (minY, maxY) = cityGrid.GetCurrentYBounds();
            
            var xCoordinate = carComesFromLeft ? minX : maxX;
            var yCoordinate = Random.Range(minY, maxY);
            cityGrid.TryGetIntersectionPosition(new Vector2Int(xCoordinate, yCoordinate), out var intersectionPosition);
            
            var currentSpawnXOffset = carComesFromLeft ? -carSpawnOffset : carSpawnOffset;
            var currentSpawnYOffset = carComesFromLeft ? -carRoadOffset : carRoadOffset;
            carSpawnPosition = intersectionPosition + new Vector3(currentSpawnXOffset, carYOffset, currentSpawnYOffset);
            carSpawnRotation = carComesFromLeft ? Quaternion.Euler(0, 90f, 0) : Quaternion.Euler(0, -90f, 0);
        }
        else
        {
            var carComesFromDown = Random.Range(0, 2) == 1;
            var (minX, maxX) = cityGrid.GetCurrentXBounds();
            var (minY, maxY) = cityGrid.GetCurrentYBounds();

            var xCoordinate = Random.Range(minX, maxX + 1);
            var yCoordinate = carComesFromDown ? minY : maxY -1;
            cityGrid.TryGetIntersectionPosition(new Vector2Int(xCoordinate, yCoordinate), out var intersectionPosition);

            var currentSpawnXOffset = carComesFromDown ? carRoadOffset : -carRoadOffset;
            var currentSpawnYOffset = carComesFromDown ? -carSpawnOffset : carSpawnOffset;
            carSpawnPosition = intersectionPosition + new Vector3(currentSpawnXOffset, carYOffset, currentSpawnYOffset);
            carSpawnRotation = carComesFromDown ? Quaternion.identity : Quaternion.Euler(0, 180f, 0);
        }

        Instantiate(carPrefab, carSpawnPosition, carSpawnRotation, transform);
        _previousCarWasHorizontal = carIsHorizontal;
    }
}
