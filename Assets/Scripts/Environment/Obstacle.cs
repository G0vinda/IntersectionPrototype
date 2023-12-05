using System;
using System.Collections.Generic;
using Character;
using UnityEngine;

namespace Environment
{
    public class Obstacle : MonoBehaviour
    {
    
        public static event Action CharacterCollided;
        
        private List<CharacterAttributes.CharColor> _allowedColors = new ();

        public void AddAllowedColor(CharacterAttributes.CharColor color)
        {
            _allowedColors.Add(color);
        }
    
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Trigger entered");
            if (!other.TryGetComponent<CharacterAttributes>(out var characterAttributes))
            {
                return;
            }

            if (!_allowedColors.Contains(characterAttributes.GetColor()))
            {
                CharacterCollided?.Invoke();
                var characterMovement = characterAttributes.GetComponent<CharacterMovement>();
                characterMovement.PushPlayerBackTunnel();
            }
        }
    }
}
