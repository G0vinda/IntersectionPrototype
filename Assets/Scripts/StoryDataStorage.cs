using UnityEngine;

public class StoryDataStorage : MonoBehaviour
{
    [SerializeField] SceneData[] levelData;
    [SerializeField] string textSceneName;
    [SerializeField] string citySceneName;
    [SerializeField] string rollSceneName;

    public SceneState GetSceneState(int level)
    {
        SceneState currentState = null;
        for (int i = levelData.Length - 1; i >= level; i--)
        {
            var textSceneData = levelData[i] as TextSceneData;
            var citySceneData = levelData[i] as CitySceneData;
            var rollSceneData = levelData[i] as RollSceneData;
            SceneState newSceneState;
            if(textSceneData != null)
            {
                newSceneState = new TextSceneState(i, textSceneName, currentState, textSceneData.text);
            }
            else if(citySceneData != null)
            {
                newSceneState = new CitySceneState(
                    i, 
                    citySceneName, 
                    currentState, 
                    citySceneData.roundTime,
                    citySceneData.goalScore, 
                    citySceneData.GetCharacterSpawnRestrictions(), 
                    citySceneData.levelType, 
                    textSceneName, 
                    citySceneData.losingTextScene);

                if(i == levelData.Length - 1)
                    newSceneState.nextScene = newSceneState;
            }
            else if(rollSceneData != null)
            {
                newSceneState = new RollSceneState(
                    i, 
                    rollSceneName, 
                    currentState, 
                    rollSceneData.GetCharacterSpawnRestrictions(), 
                    rollSceneData.GetPredeterminedCharacter(), 
                    textSceneName, 
                    rollSceneData.predeterminedTextScene, 
                    rollSceneData.normalTextScene);

                if(i == levelData.Length -2)
                {
                    ((RollSceneState)newSceneState).goDirectlyToLevelScene = true;
                    currentState.nextScene = newSceneState;
                }
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
        return levelData.Length - 2;
    }
}
