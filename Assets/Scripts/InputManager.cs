using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using GG.Infrastructure.Utils.Swipe;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static event Action<Vector2Int> RegisteredMoveInput;
    
    private SwipeListener _swipeListener;

    private void Awake()
    {
        _swipeListener = GetComponent<SwipeListener>();
    }

    private void OnEnable()
    {
        _swipeListener.OnSwipe.AddListener(OnSwipe);
    }

    private void OnDisable()
    {
        _swipeListener.OnSwipe.RemoveListener(OnSwipe);
    }

    private void OnSwipe(string swipeType)
    {
        //Debug.Log($"SwipeType is {swipeType}");
        switch (swipeType)
        {
            case "Up":
                RegisteredMoveInput?.Invoke(Vector2Int.up);
                break;
            case "Left":
                RegisteredMoveInput?.Invoke(Vector2Int.left);
                break;
            case "Right":
                RegisteredMoveInput?.Invoke(Vector2Int.right);
                break;
            case "Down":
                RegisteredMoveInput?.Invoke(Vector2Int.down);
                break;
        }
    }
}
