using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextSceneState : SceneState
{
    public string text;

    public TextSceneState(int id, string unitySceneName, SceneState nextScene, string text) : base(id, unitySceneName, nextScene)
    {
        this.text = text;
    }

    public override void OnBackClicked(SceneState previousState)
    {
        flowManager.GoBackToTitleMenu();
    }

    public override void OnContinueClicked()
    {
        if(nextScene != null)
        {
            flowManager.LoadScene(nextScene);
        }
        else
        {
            flowManager.GoBackToTitleMenu();
        }
    }
}
