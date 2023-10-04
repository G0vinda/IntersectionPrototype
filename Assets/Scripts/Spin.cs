using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] private Vector3 spinVector;
    [SerializeField] private float spinSpeed;
    
    void Update()
    {
        transform.Rotate(spinVector * spinSpeed * Time.deltaTime);    
    }
}
