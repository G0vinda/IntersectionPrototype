using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextSceneState : SceneState
{
    public string text {get; set;}

    private const string shapeLabel = "%PlayerShape%";
    private const string colorLabel = "%PlayerColor%";
    private const string patternLabel = "%PlayerPattern%";
    private const string scoreLabel ="%PlayerScore%";
    private bool _isSecondPage;


    public TextSceneState(int id, string unitySceneName, SceneState nextScene, string text, TextSceneData secondPage = null, bool isSecondPage = false) : base(id, unitySceneName, nextScene)
    {
        this.text = text;
        _isSecondPage = isSecondPage;

        if(secondPage != null)
        {
            var secondPageScene = new TextSceneState(0, unitySceneName, nextScene, secondPage.text, null, true);
            this.nextScene = secondPageScene;
        }
    }

    public string GetText(CharacterAttributes characterAttributes, int score)
    {        
        string textWithValues = (string)text.Clone();
        if(characterAttributes != null)
        {
            var shapeTerm = characterAttributes.shape switch {
                CharacterAttributes.Shape.Cube => "square",
                CharacterAttributes.Shape.Pyramid => "triangle",
                CharacterAttributes.Shape.Sphere => "circle",
                _ => throw new ArgumentException()
            };

            var colorTerm = characterAttributes.color switch {
                CharacterAttributes.Color.Blue => "blue",
                CharacterAttributes.Color.Red => "red",
                CharacterAttributes.Color.Yellow => "yellow",
                _ => throw new ArgumentException()
            };

            var patternTerm = characterAttributes.pattern switch {
                CharacterAttributes.Pattern.Check => "with a check pattern",
                CharacterAttributes.Pattern.Lined => "with lines",
                CharacterAttributes.Pattern.None => "without pattern",
                _ => throw new ArgumentException()
            };

            textWithValues = textWithValues.Replace(shapeLabel, shapeTerm).Replace(colorLabel, colorTerm).Replace(patternLabel, patternTerm);
        }

        return textWithValues.Replace(scoreLabel, score.ToString());
    }

    public override void OnBackClicked(SceneState previousState)
    {
        if(_isSecondPage)
        {
            flowManager.LoadScene(previousState);
            return;
        }

        flowManager.GoBackToTitleMenu();
    }

    public override void OnContinueClicked()
    {
        flowManager.LoadScene(nextScene);
    }
}
