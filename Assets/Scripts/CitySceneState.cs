using Character;

public class CitySceneState : SceneState
{
    public int roundTime;
    public int goalScore;
    public CharacterAttributes.SpawnRestrictions spawnRestrictions;
    public LevelType levelType;
    public bool playerLost;

    private TextSceneState _loosingTextScene;

    public CitySceneState(int id, string unitySceneName, SceneState nextScene, int roundTime, int goalScore, CharacterAttributes.SpawnRestrictions spawnRestrictions, LevelType levelType, string textSceneName, TextSceneData losingTextScene) : base(id, unitySceneName, nextScene)
    {
        this.roundTime = roundTime;
        this.goalScore = goalScore;
        this.spawnRestrictions = spawnRestrictions;
        this.levelType = levelType;

        if(losingTextScene == null)
            return;

        _loosingTextScene = new TextSceneState(0, textSceneName, null, losingTextScene.text);
    }

    public override void OnBackClicked(SceneState previousState)
    {
        flowManager.GoBackToTitleMenu();
    }

    public override void OnContinueClicked()
    {
        SceneState sceneToLoad;
        if(playerLost)
        {
            _loosingTextScene.nextScene = flowManager.lastRollSceneState;
            sceneToLoad = _loosingTextScene;
            playerLost = false;
        }
        else
        {
            sceneToLoad = nextScene;
        }

        flowManager.LoadScene(sceneToLoad);
    }

    public enum LevelType
    {
        Normal,
        Tutorial,
        HighScore
    }
}
