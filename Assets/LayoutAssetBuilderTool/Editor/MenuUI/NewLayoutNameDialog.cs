using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR

using LayoutAssetBuilderTool;

public class NewLayoutNameDialog : MonoBehaviour
{
    [SerializeField] Button createButton;
    [SerializeField] LayoutAssetBuilderMenu layoutAssetBuilderMenu;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] DifficultyIcon difficultyIcon;

    private int _layoutDifficulty;

    public void ResetDialog()
    {
        _layoutDifficulty = 0;
        difficultyIcon.SetDifficulty(0);
    }

    public void InputTextChanged()
    {
        createButton.interactable = inputField.text.Length > 0;
    }

    public void ChangeDifficulty(int difficulty)
    {
        _layoutDifficulty = difficulty;
        difficultyIcon.SetDifficulty(difficulty);
    }

    public void CreateButtonPressed()
    {
        layoutAssetBuilderMenu.OpenNewLayout(inputField.text, _layoutDifficulty);
    }
}

#endif
