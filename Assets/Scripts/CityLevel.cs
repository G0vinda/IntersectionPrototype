using System.Collections;
using System.Collections.Generic;
using Character;
using Cinemachine;
using UnityEngine;
using UI;
using TMPro;

public class CityLevel : MonoBehaviour
{
    [SerializeField] CharacterRollDialog characterRollDialog;
    [SerializeField] CityGridCreator cityGrid;
    [SerializeField] CharacterMovement playerPrefab;
    [SerializeField] Vector2Int playerStartCoordinates;
    [SerializeField] CinemachineVirtualCamera cam;
    [SerializeField] GameObject inGameUI;
    [SerializeField] UIRoundTimer roundTimerUI;
    [SerializeField] GameObject testContinueButton;
    [SerializeField] HighScoreNamePrompt highScoreNamePrompt;
    [SerializeField] UIHighScoreEntryList highScoreList;

    private int _roundTime;
    private CharacterAttributes.SpawnRestrictions _spawnRestrictions;
    private CitySceneState.LevelType _levelType;
    private CharacterMovement _playerMovement;
    private InputManager _inputManager;
    private ScoringSystem _scoringSystem;

    void Awake()
    {
        _inputManager = GetComponent<InputManager>();
        _scoringSystem = GetComponent<ScoringSystem>();
        var info = FlowManager.Instance.GetCityLevelInfo();
        _roundTime = info.roundTime;
        _spawnRestrictions = info.spawnRestrictions;
        _levelType = info.levelType;
    }

    public void OnEnable()
    {
        Time.timeScale = 1f;
        characterRollDialog.StartRoll(this, _spawnRestrictions);
    }

    public void StartLevel(CharacterAttributes playerAttributes)
    {
        characterRollDialog.gameObject.SetActive(false);
        cityGrid.CreateNewCityGrid(_spawnRestrictions, _levelType != CitySceneState.LevelType.Tutorial);

        cityGrid.TryGetIntersectionPosition(playerStartCoordinates, out var playerStartPosition);
        _playerMovement = Instantiate(playerPrefab);
        _playerMovement.GetComponent<CharacterAppearance>().SetAttributes(playerAttributes);
        _playerMovement.Initialize(playerStartPosition, playerStartCoordinates, cityGrid, _scoringSystem);

        cam.Follow = _playerMovement.transform;
        inGameUI.SetActive(true);
        _inputManager.enabled = true;

        if(_levelType == CitySceneState.LevelType.Tutorial)
        {

        }
        else
        {
            StartCoroutine(RoundTimer());
        }
    }

    private IEnumerator RoundTimer()
    {
        var currentTime = (float)_roundTime;
        roundTimerUI.maxTime = currentTime;
        while(currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            roundTimerUI.UpdateTimerUI(currentTime);
            yield return null;
        }

        _inputManager.enabled = false;
        Time.timeScale = 0f;
        inGameUI.SetActive(false);

        if(_levelType == CitySceneState.LevelType.Normal)
        {
            testContinueButton.SetActive(true);
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
        public CharacterAttributes.SpawnRestrictions spawnRestrictions;
        public CitySceneState.LevelType levelType;
    }
}
