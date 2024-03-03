using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CitySceneState : SceneState
{
    public int roundTime;
    public CharacterAttributes.SpawnRestrictions spawnRestrictions;
    public LevelType levelType;

    public CitySceneState(int id, string unitySceneName, SceneState nextScene, int roundTime, CharacterAttributes.SpawnRestrictions spawnRestrictions, LevelType levelType) : base(id, unitySceneName, nextScene)
    {
        this.roundTime = roundTime;
        this.spawnRestrictions = spawnRestrictions;
        this.levelType = levelType;
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
    }

    public enum LevelType
    {
        Normal,
        Tutorial,
        HighScore
    }
}
