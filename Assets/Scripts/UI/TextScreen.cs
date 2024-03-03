using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textObject;

    void Awake()
    {
        textObject.text = FlowManager.Instance.GetText();    
    }

    public void OnContinueClick()
    {
        FlowManager.Instance.ContinueClicked();
    }

    public void OnBackClick()
    {
        FlowManager.Instance.BackClicked();
    }
}
