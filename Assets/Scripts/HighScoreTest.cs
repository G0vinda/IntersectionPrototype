using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreTest : MonoBehaviour
{
    [SerializeField] FlowManager flowManagerPrefab;
    [SerializeField] StoryDataStorage storyDataStorage;
    
    void Start()
    {
        ProgressValues.checkPointScene = storyDataStorage.GetSceneState(storyDataStorage.GetHighScoreLevelIndex());
        var flowManager = Instantiate(flowManagerPrefab);
        
        flowManager.LoadScene(ProgressValues.checkPointScene);
    }
}
