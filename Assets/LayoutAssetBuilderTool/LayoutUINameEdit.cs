using System;
using System.Collections;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR

[RequireComponent(typeof(TMP_InputField))]
public class LayoutUINameEdit : MonoBehaviour
{
    public Action<string> NameEditSubmitted;
    public Action NameEditCanceled;
    public string text{
        get => _inputField.text;
        private set => _inputField.text = value;
    }

    private TMP_InputField _inputField;
    private bool _selected;

    void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
    }

    public void StartEdit(string name)
    {
        _inputField.text = name;
        _inputField.ActivateInputField();
        _selected = true;
        StartCoroutine(Editing());
    }

    public void OnDeselect()
    {
        _selected = false;
        NameEditCanceled?.Invoke();
    }

    private IEnumerator Editing()
    {
        while (_selected)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                NameEditSubmitted?.Invoke(_inputField.text);
                break;
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                NameEditCanceled?.Invoke();
                break;
            }
            
            yield return null;
        }
    }
}

#endif