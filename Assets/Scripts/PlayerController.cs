using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    
    private Input _input;
    private Camera _cam;
    private NavMeshAgent _navMeshAgent;
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _input = new Input();
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.MovePress.performed += ProcessMoveInput;
    }

    private void OnDisable()
    {
        _input.Disable();
        _input.Player.MovePress.performed -= ProcessMoveInput;
    }

    private void ProcessMoveInput(InputAction.CallbackContext context)
    {
        Debug.Log("Input detected");
        var inputPosition = _input.Player.MovePosition.ReadValue<Vector2>();
        var ray = _cam.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out var hit, 100, groundLayer))
        {
            _navMeshAgent.SetDestination(hit.point);
        }
    }
}
