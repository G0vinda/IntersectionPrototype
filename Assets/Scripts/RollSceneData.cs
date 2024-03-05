using UnityEngine;
using Character;

[CreateAssetMenu(fileName = "NewRollSceneData", menuName = "SceneData/RollSceneData", order = 100)]
public class RollSceneData : SceneData
{
    [Header("Spawn Restrictions")]
    public bool useOnlyOneShape;
    public CharacterAttributes.CharShape onlyShape;
    public bool useOnlyOneColor;
    public CharacterAttributes.CharColor onlyColor;
    public bool useOnlyOnePattern;
    public CharacterAttributes.CharPattern onlyPattern;

    [Header("First Predetermined Character")]
    public CharacterAttributes.CharShape shape;
    public CharacterAttributes.CharColor color;
    public CharacterAttributes.CharPattern pattern;

    public bool usePredeterminedCharacter;

    [Header("Following text Scenes")]
    public TextSceneData predeterminedTextScene;
    public TextSceneData normalTextScene;
    public bool goDirectlyToLevelScene;

    public CharacterAttributes.SpawnRestrictions GetCharacterSpawnRestrictions()
    {
        return new CharacterAttributes.SpawnRestrictions(
            useOnlyOneShape ? onlyShape : (CharacterAttributes.CharShape)(-1),
            useOnlyOneColor ? onlyColor : (CharacterAttributes.CharColor)(-1),
            useOnlyOnePattern ? onlyPattern : (CharacterAttributes.CharPattern)(-1)
        );
    }

    public CharacterAttributes GetPredeterminedCharacter()
    {
        if(!usePredeterminedCharacter)
            return null;
        
        return new CharacterAttributes(shape, color, pattern);
    }
}
