using System.Collections;
using System.Collections.Generic;
using LayoutAssetBuilderTool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewLayoutNameDialog : MonoBehaviour
{
    [SerializeField] Button createButton;
    [SerializeField] LayoutAssetBuilderMenu layoutAssetBuilderMenu;
    [SerializeField] TMP_InputField inputField;

    public void InputTextChanged()
    {
        createButton.interactable = inputField.text.Length > 0;
    }

    public void CreateButtonPressed()
    {
        layoutAssetBuilderMenu.OpenNewLayout(inputField.text);
    }
}
