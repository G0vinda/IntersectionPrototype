using System;
using System.Collections;
using System.Collections.Generic;
using GG.Infrastructure.Utils.Swipe;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    
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
        Debug.Log($"SwipeType is {swipeType}");
        switch (swipeType)
        {
            case "Up":
                characterMovement.MovePlayer(Vector2Int.up);
                break;
            case "Left":
                characterMovement.MovePlayer(Vector2Int.left);
                break;
            case "Right":
                characterMovement.MovePlayer(Vector2Int.right);
                break;
        }
    }
}
