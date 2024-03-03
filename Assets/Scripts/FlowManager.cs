using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowManager : MonoBehaviour
{
    public static FlowManager Instance;

    [SerializeField] private string titleMenuSceneName;

    private SceneState _currentSceneState;
    private SceneState _previousSceneState;

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
    }

    public CityLevel.Info GetCityLevelInfo()
    {
        if(_currentSceneState is CitySceneState)
        {
            var info = new CityLevel.Info
            {
                roundTime = ((CitySceneState)_currentSceneState).roundTime,
                spawnRestrictions = ((CitySceneState)_currentSceneState).spawnRestrictions,
                levelType = ((CitySceneState)_currentSceneState).levelType
            };
            return info;
        }

        throw new System.Exception("Tried to read city level info from scene that is not cityScene.");
    }

    public string GetText()
    {
        if(_currentSceneState is TextSceneState)
        {
            return ((TextSceneState)_currentSceneState).text;
        }

        throw new System.Exception("Tried to read text from scene that is not textScene.");
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
        ProgressValues.lastLevelId = _currentSceneState.nextScene != null ? _currentSceneState.nextScene.id : _currentSceneState.id;
    }
}
