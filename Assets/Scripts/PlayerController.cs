using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject clickEffectPrefab;
    
    private Input _input;
    private Camera _cam;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private bool _isWalking;
    private int _walkForwardHash;
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _walkForwardHash = Animator.StringToHash("Walk Forward");
        _animator = GetComponent<Animator>();
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

    private void Update()
    {
        if(!_isWalking)
            return;

        if (!_navMeshAgent.hasPath)
        {
            _isWalking = false;
            _animator.SetBool(_walkForwardHash, false);
        }
    }

    private void ProcessMoveInput(InputAction.CallbackContext context)
    {
        
        var inputPosition = _input.Player.MovePosition.ReadValue<Vector2>();
        var ray = _cam.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out var hit, 1000, groundLayer))
        {
            _navMeshAgent.SetDestination(hit.point);
            Instantiate(clickEffectPrefab, hit.point, Quaternion.Euler(new Vector3(-90, 0 , 0)));
            _isWalking = true;
            _animator.SetBool(_walkForwardHash, true);
        }
    }
}
