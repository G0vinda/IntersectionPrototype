using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;

public class Park : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent<PlayerMovement>(out var characterMovement))
            return;
        
        characterMovement.SlowCharacter();
    }
}
