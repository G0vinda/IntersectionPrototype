using Character;

public class RollSceneState : SceneState
{
    public CharacterAttributes.SpawnRestrictions spawnRestrictions {get; private set;}
    public CharacterAttributes predeterminedAttributes {
        get => _usedPredetermindedAttributes ? null : _predeterminedAttributes;
        private set 
        { 
            _predeterminedAttributes = value;
        }
    }
    public bool goDirectlyToLevelScene;

    private CharacterAttributes _predeterminedAttributes;
    private bool _usedPredetermindedAttributes = false;
    private TextSceneState _predeterminedTextScene;
    private TextSceneState _normalTextScene;

    public RollSceneState(
        int id, 
        string unitySceneName, 
        SceneState nextScene,
        CharacterAttributes.SpawnRestrictions spawnRestrictions,
        CharacterAttributes predeterminedAttributes,
        string unityTextSceneName,
        TextSceneData predeterminedTextScene,
        TextSceneData normalTextScene) : base(id, unitySceneName, nextScene)
    {
        this.spawnRestrictions = spawnRestrictions;
        _predeterminedAttributes = predeterminedAttributes;   
        
        if(predeterminedTextScene != null)
            _predeterminedTextScene = new TextSceneState(0, unityTextSceneName, nextScene, predeterminedTextScene.text);

        if(normalTextScene != null)
            _normalTextScene = new TextSceneState(0, unityTextSceneName, nextScene, normalTextScene.text);
    }

    public override void OnBackClicked(SceneState previousState)
    {
        throw new System.NotImplementedException();
    }

    public override void OnContinueClicked()
    {
        SceneState sceneToLoad = _usedPredetermindedAttributes ? _normalTextScene : _predeterminedTextScene;
        if(goDirectlyToLevelScene)
            sceneToLoad = nextScene;
        
        _usedPredetermindedAttributes = true;
        flowManager.LoadScene(sceneToLoad);
    }
}
