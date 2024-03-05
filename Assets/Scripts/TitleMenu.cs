using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] FlowManager flowManagerPrefab;
    [SerializeField] StoryDataStorage storyDataStorage;
    [SerializeField] Button playButton;

    void Start()
    {
        playButton.interactable = true;
        
        if(!LootLockerManager.Instance.loggedIn)
        {
            playButton.interactable = false;
            LootLockerManager.Instance.playerLoggedIn += EnablePlayButtonOnLogin;
            LootLockerManager.Instance.Login();
        }
    }

    private void EnablePlayButtonOnLogin()
    {
        playButton.interactable = true;
        LootLockerManager.Instance.playerLoggedIn -= EnablePlayButtonOnLogin;
    }

    public void StartStory()
    {
        var firstScene = ProgressValues.checkPointScene != null ? ProgressValues.checkPointScene : storyDataStorage.GetSceneState(0);
        
        var flowManager = Instantiate(flowManagerPrefab);
        flowManager.LoadScene(firstScene);
    }

    public void GoToHighScores()
    {
        SceneManager.LoadScene("HighScoreScene");
    }
}
