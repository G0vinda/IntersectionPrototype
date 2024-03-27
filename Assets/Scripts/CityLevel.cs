using System.Collections;
using Character;
using Cinemachine;
using UnityEngine;
using UI;
using System.Collections.Generic;

public class CityLevel : MonoBehaviour
{
    [SerializeField] CityGridCreator cityGrid;
    [SerializeField] CharacterMovement playerPrefab;
    [SerializeField] Vector2Int playerStartCoordinates;
    [SerializeField] CameraController cameraController;
    [SerializeField] GameObject inGameUI;
    [SerializeField] UIRoundTimer roundTimerUI;
    [SerializeField] GameObject testContinueButton;
    [SerializeField] HighScoreNamePrompt highScoreNamePrompt;
    [SerializeField] UIHighScoreEntryList highScoreList;
    [SerializeField] GameObject winObject;
    [SerializeField] GameObject timesUpObject;

    private int _roundTime;
    private CharacterAttributes.SpawnRestrictions _spawnRestrictions;
    private CitySceneState.LevelType _levelType;
    private CharacterMovement _playerMovement;
    private CharacterAttributes _playerAttributes;
    private InputManager _inputManager;
    private ScoringSystem _scoringSystem;
    private int _goalScore;

    void Awake()
    {
        _inputManager = GetComponent<InputManager>();
        _scoringSystem = GetComponent<ScoringSystem>();
        var info = FlowManager.Instance.GetCityLevelInfo();
        _roundTime = info.roundTime;
        _goalScore = info.goalScore;
        _spawnRestrictions = info.spawnRestrictions;
        _levelType = info.levelType;
        _playerAttributes = info.playerAttributes;
    }

    public void Start()
    {
        Time.timeScale = 1f;
        StartLevel();
    }

    public void StartLevel()
    {
        cityGrid.CreateNewCityGrid(_spawnRestrictions, cameraController, _levelType != CitySceneState.LevelType.Tutorial);

        cityGrid.TryGetIntersectionPosition(playerStartCoordinates, out var playerStartPosition);
        _playerMovement = Instantiate(playerPrefab);
        _playerMovement.GetComponent<CharacterAppearance>().SetAttributes(_playerAttributes);
        _playerMovement.Initialize(playerStartPosition, playerStartCoordinates, cityGrid, _scoringSystem);

        cameraController.SetCamTarget(_playerMovement.transform);
        inGameUI.SetActive(true);
        _inputManager.enabled = true;

        if(_levelType != CitySceneState.LevelType.HighScore)
        {
            _scoringSystem.ScoreChanged += CheckIfGoalScoreReached;
        }

        if(_levelType != CitySceneState.LevelType.Tutorial)
        {
            StartCoroutine(RoundTimer());
        }
    }

    private IEnumerator RoundTimer()
    {
        var currentTime = (float)_roundTime;
        roundTimerUI.maxTime = currentTime;
        roundTimerUI.SetTextActive();
        while(currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            roundTimerUI.UpdateTimerUI(currentTime);
            yield return null;
        }

        FinishRound();
    }
    
    private void CheckIfGoalScoreReached(int newScore)
    {
        if(newScore < _goalScore)
            return;
        
        _scoringSystem.ScoreChanged -= CheckIfGoalScoreReached;
        FinishRound();
    }

    private void FinishRound()
    {
        _inputManager.enabled = false;
        Time.timeScale = 0f;
        inGameUI.SetActive(false);

        if(_levelType != CitySceneState.LevelType.HighScore)
        {
            testContinueButton.SetActive(true);
            if(_scoringSystem.score < _goalScore)
            {
                FlowManager.Instance.PlayerLostRound(_scoringSystem.score);
                timesUpObject.SetActive(true);
                winObject.SetActive(false);
            }
            else
            {
                winObject.SetActive(true);
                timesUpObject.SetActive(false);
            }
        }
        else
        {
            var playerAttributes = _playerMovement.GetComponent<CharacterAppearance>().GetAttributes();
            var highScoreData = new ScoringSystem.HighScoreEntryData(_scoringSystem.score, "defaultName", playerAttributes);
            highScoreNamePrompt.gameObject.SetActive(true);
            highScoreNamePrompt.Initialize(this, highScoreData);
        }
        FlowManager.Instance.SaveProgress();
    }

    public void ShowHighScoreList(ScoringSystem.HighScoreEntryData highScoreDataToBeMarked)
    {
        highScoreList.gameObject.SetActive(true);
        highScoreList.Initialize(highScoreDataToBeMarked);
    }

    public void ContinueClicked()
    {
        FlowManager.Instance.ContinueClicked();
    }

    public class Info
    {
        public int roundTime;
        public int goalScore;
        public CharacterAttributes playerAttributes;
        public CharacterAttributes.SpawnRestrictions spawnRestrictions;
        public CitySceneState.LevelType levelType;
    }
}
