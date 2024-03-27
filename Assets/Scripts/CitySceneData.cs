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
    public CharacterAttributes.Shape onlyShape;
    public bool useOnlyOneColor;
    public CharacterAttributes.Color onlyColor;
    public bool useOnlyOnePattern;
    public CharacterAttributes.Pattern onlyPattern;

    public CitySceneState.LevelType levelType;

    public CharacterAttributes.SpawnRestrictions GetCharacterSpawnRestrictions()
    {
        return new CharacterAttributes.SpawnRestrictions(
            useOnlyOneShape ? onlyShape : (CharacterAttributes.Shape)(-1),
            useOnlyOneColor ? onlyColor : (CharacterAttributes.Color)(-1),
            useOnlyOnePattern ? onlyPattern : (CharacterAttributes.Pattern)(-1)
        );
    }
}
