using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextSceneState : SceneState
{
    private const string shapeLabel = "%PlayerShape%";
    private const string colorLabel = "%PlayerColor%";
    private const string patternLabel = "%PlayerPattern%";
    private string _text;

    public TextSceneState(int id, string unitySceneName, SceneState nextScene, string text) : base(id, unitySceneName, nextScene)
    {
        _text = text;
    }

    public string GetText(CharacterAttributes characterAttributes)
    {
        if(characterAttributes == null)
            return _text;

        var shapeTerm = characterAttributes.shape switch {
            CharacterAttributes.CharShape.Cube => "square",
            CharacterAttributes.CharShape.Pyramid => "triangle",
            CharacterAttributes.CharShape.Sphere => "circle",
            _ => throw new ArgumentException()
        };

        var colorTerm = characterAttributes.color switch {
            CharacterAttributes.CharColor.Blue => "blue",
            CharacterAttributes.CharColor.Red => "red",
            CharacterAttributes.CharColor.Yellow => "yellow",
            _ => throw new ArgumentException()
        };

        var patternTerm = characterAttributes.pattern switch {
            CharacterAttributes.CharPattern.Check => "with a check pattern",
            CharacterAttributes.CharPattern.Lined => "with lines",
            CharacterAttributes.CharPattern.None => "without pattern",
            _ => throw new ArgumentException()
        };

        return _text.Replace(shapeLabel, shapeTerm).Replace(colorLabel, colorTerm).Replace(patternLabel, patternTerm);
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
        else
        {
            flowManager.GoBackToTitleMenu();
        }
    }
}
