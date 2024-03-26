using TMPro;
using UnityEngine;

#if UNITY_EDITOR

using LayoutAssetBuilderTool;

public class DeleteLayoutConfirmDialog : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI headerText;
    [SerializeField] LayoutAssetBuilderMenu layoutAssetBuilderMenu;

    private const string confirmPreText = "Are you sure you want to delete ";
    private int _idToDelete;

    public void Open(string layoutNameToDelete, int idToDelete)
    {
        headerText.text = $"{confirmPreText}{layoutNameToDelete}?";
        _idToDelete = idToDelete;
        gameObject.SetActive(true);
    }

    public void DeleteButtonPressed()
    {
        layoutAssetBuilderMenu.DeleteLayout(_idToDelete);
    }
}

#endif
