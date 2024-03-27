using System;
using System.Collections.Generic;
using Character;
using UnityEngine;

namespace Environment
{
    public class Obstacle : MonoBehaviour
    {
    
        public static event Action CharacterCollided;
        
        private List<CharacterAttributes.Color> _allowedColors = new ();

        public void AddAllowedColor(CharacterAttributes.Color color)
        {
            _allowedColors.Add(color);
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<CharacterMovement>(out var characterMovement))
            {
                return;
            }

            var characterAppearance = characterMovement.GetComponent<CharacterAppearance>();

            if (!_allowedColors.Contains(characterAppearance.GetAttributes().color))
            {
                CharacterCollided?.Invoke();
                characterMovement.PushPlayerBackObstacle();
            }
        }
    }
}
