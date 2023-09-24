using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] private float speed;

    private Vector3 _direction;

    private void Start()
    {
        _direction = transform.forward;
    }

    void Update()
    {
        transform.position += _direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        UIPromptSystem.Instance.ShowCarHitPrompt();
    }
}