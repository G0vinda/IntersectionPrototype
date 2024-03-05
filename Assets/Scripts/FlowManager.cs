using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowManager : MonoBehaviour
{
    public static FlowManager Instance;

    [SerializeField] private string titleMenuSceneName;

    public CharacterAttributes currentCharacterAttributes {get; set;}
    public SceneState lastRollSceneState {get; private set;}

    private SceneState _currentSceneState;
    private SceneState _previousSceneState;
    private int _lastLosingScore;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(SceneState sceneState)
    {
        _previousSceneState = _currentSceneState;
        _currentSceneState = sceneState;
        sceneState.flowManager = this;
        SceneManager.LoadScene(sceneState.unitySceneName);            

        if(_currentSceneState is RollSceneState)
            lastRollSceneState = _currentSceneState;
    }

    public CityLevel.Info GetCityLevelInfo()
    {
        if(_currentSceneState is CitySceneState)
        {
            var info = new CityLevel.Info
            {
                roundTime = ((CitySceneState)_currentSceneState).roundTime,
                goalScore = ((CitySceneState)_currentSceneState).goalScore,
                spawnRestrictions = ((CitySceneState)_currentSceneState).spawnRestrictions,
                levelType = ((CitySceneState)_currentSceneState).levelType,
                playerAttributes = currentCharacterAttributes
            };
            return info;
        }

        throw new System.Exception("Tried to read city level info from scene that is not cityScene.");
    }

    public RollTrigger.Info GetRollTriggerInfo()
    {
        if(_currentSceneState is RollSceneState)
        {
            var info = new RollTrigger.Info
            {
                spawnRestrictions = ((RollSceneState)_currentSceneState).spawnRestrictions,
                predeterminedAttributes = ((RollSceneState)_currentSceneState).predeterminedAttributes
            };
            return info;
        }

        throw new System.Exception("Tried to read roll scene info from scene that is not rollScene.");
    }

    public string GetText()
    {
        if(_currentSceneState is TextSceneState)
        {
            return ((TextSceneState)_currentSceneState).GetText(currentCharacterAttributes, _lastLosingScore);
        }

        throw new System.Exception("Tried to read text from scene that is not textScene.");
    }

    public void PlayerLostRound(int losingScore)
    {
        ((CitySceneState)_currentSceneState).playerLost = true;
        _lastLosingScore = losingScore;
    }

    public void GoBackToTitleMenu()
    {
        SceneManager.LoadScene(titleMenuSceneName);
    }

    public void BackClicked()
    {
        _currentSceneState.OnBackClicked(_previousSceneState);
    }

    public void ContinueClicked()
    {
        _currentSceneState.OnContinueClicked();
    }

    public void SaveProgress()
    {
        ProgressValues.checkPointScene = _currentSceneState.nextScene;
    }
}
