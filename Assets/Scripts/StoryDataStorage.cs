using UnityEngine;

public class StoryDataStorage : MonoBehaviour
{
    [SerializeField] LevelData[] levelData;
    [SerializeField] string textSceneName;
    [SerializeField] string citySceneName;

    public SceneState GetSceneState(int level)
    {
        SceneState currentState = null;
        for (int i = levelData.Length - 1; i >= level; i--)
        {
            var textSceneData = levelData[i] as TextSceneData;
            var citySceneData = levelData[i] as CitySceneData;
            SceneState newSceneState;
            if(textSceneData != null)
            {
                newSceneState = new TextSceneState(i, textSceneName, currentState, textSceneData.text);
            }
            else if(citySceneData != null)
            {
                newSceneState = new CitySceneState(i, citySceneName, currentState, citySceneData.roundTime, citySceneData.GetCharacterSpawnRestrictions(), citySceneData.levelType);
                if(i == levelData.Length - 1)
                    newSceneState.nextScene = newSceneState;
            }
            else
            {
                throw new System.Exception("Error on Reading LevelData. It is neither of type textSceneData nor citySceneData");
            }

            currentState = newSceneState;
        }

        return currentState;
    }

    public int GetHighScoreLevelIndex()
    {
        return levelData.Length - 1;
    }
}
