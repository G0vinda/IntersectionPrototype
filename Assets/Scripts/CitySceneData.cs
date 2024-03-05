using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;

[CreateAssetMenu(fileName = "NewCitySceneData", menuName = "SceneData/CitySceneData", order = 100)]
public class CitySceneData : SceneData
{
    public TextSceneData losingTextScene;
    public int roundTime;
    public int goalScore;

    [Header("Spawn Restrictions")]
    public bool useOnlyOneShape;
    public CharacterAttributes.CharShape onlyShape;
    public bool useOnlyOneColor;
    public CharacterAttributes.CharColor onlyColor;
    public bool useOnlyOnePattern;
    public CharacterAttributes.CharPattern onlyPattern;

    public CitySceneState.LevelType levelType;

    public CharacterAttributes.SpawnRestrictions GetCharacterSpawnRestrictions()
    {
        return new CharacterAttributes.SpawnRestrictions(
            useOnlyOneShape ? onlyShape : (CharacterAttributes.CharShape)(-1),
            useOnlyOneColor ? onlyColor : (CharacterAttributes.CharColor)(-1),
            useOnlyOnePattern ? onlyPattern : (CharacterAttributes.CharPattern)(-1)
        );
    }
}
