using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneState
{
    public int id;
    public string unitySceneName;
    public SceneState nextScene;
    public FlowManager flowManager;

    public abstract void OnBackClicked(SceneState previousState);
    public abstract void OnContinueClicked();

    public SceneState(int id, string unitySceneName, SceneState nextScene)
    {
        this.id = id;
        this.unitySceneName = unitySceneName;
        this.nextScene = nextScene;
    }
}
