using System.Collections;
using Character;
using Cinemachine;
using UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float roundTime;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private UIRoundTimer roundTimerUI;
    [SerializeField] private ScoringSystem scoringSystem;
    [SerializeField] private CityGridCreator cityGrid;
    [SerializeField] private TrafficManager trafficManager;
    [SerializeField] private CharacterMovement characterPrefab;
    [SerializeField] private Vector2Int characterStartCoordinates;
    [SerializeField] private CinemachineVirtualCamera camera;

    private float _currentTime;
    private CharacterMovement _character;
    
    void Start()
    {
        StartRound();
    }

    private void StartRound()
    {
        cityGrid.CreateNewCityGrid();
        trafficManager.RestartTraffic();
        Time.timeScale = 1.0f;
        inputManager.enabled = true;

        if (_character != null)
        {
            Destroy(_character.gameObject);   
        }
        cityGrid.TryGetIntersectionPosition(characterStartCoordinates, out var characterStartPosition);
        _character = Instantiate(characterPrefab);
        var colorIndex = Random.Range(0, 3);
        var meshIndex = Random.Range(0, 3);
        _character.Initialize(characterStartPosition, characterStartCoordinates, cityGrid, scoringSystem, colorIndex, meshIndex);
        camera.Follow = _character.transform;
        
        StartCoroutine(RoundTimer());
    }
    
    public void RestartRound()
    {
        StartRound();
    }

    private IEnumerator RoundTimer()
    {
        _currentTime = roundTime;
        roundTimerUI.EnableTimer(_currentTime);
        while (_currentTime > 0)
        {
            _currentTime -= Time.deltaTime;
            roundTimerUI.UpdateTimerUI(_currentTime);
            yield return null;
        }
        
        // process End of turn
        inputManager.enabled = false;
        Time.timeScale = 0f;
        scoringSystem.ShowHighScoreList();
    }
}
