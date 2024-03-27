using UnityEngine;
using Character;

[CreateAssetMenu(fileName = "NewRollSceneData", menuName = "SceneData/RollSceneData", order = 100)]
public class RollSceneData : SceneData
{
    [Header("Spawn Restrictions")]
    public bool useOnlyOneShape;
    public CharacterAttributes.Shape onlyShape;
    public bool useOnlyOneColor;
    public CharacterAttributes.Color onlyColor;
    public bool useOnlyOnePattern;
    public CharacterAttributes.Pattern onlyPattern;

    [Header("First Predetermined Character")]
    public CharacterAttributes.Shape shape;
    public CharacterAttributes.Color color;
    public CharacterAttributes.Pattern pattern;

    public bool usePredeterminedCharacter;

    [Header("Following text Scenes")]
    public TextSceneData predeterminedTextScene;
    public TextSceneData normalTextScene;
    public bool goDirectlyToLevelScene;

    public CharacterAttributes.SpawnRestrictions GetCharacterSpawnRestrictions()
    {
        return new CharacterAttributes.SpawnRestrictions(
            useOnlyOneShape ? onlyShape : (CharacterAttributes.Shape)(-1),
            useOnlyOneColor ? onlyColor : (CharacterAttributes.Color)(-1),
            useOnlyOnePattern ? onlyPattern : (CharacterAttributes.Pattern)(-1)
        );
    }

    public CharacterAttributes GetPredeterminedCharacter()
    {
        if(!usePredeterminedCharacter)
            return null;
        
        return new CharacterAttributes(shape, color, pattern);
    }
}
