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
            if (!other.TryGetComponent<CharacterAppearance>(out var characterAppearance))
            {
                return;
            }

            if (!_allowedColors.Contains(characterAppearance.GetAttributes().color))
            {
                CharacterCollided?.Invoke();
                var characterMovement = characterAppearance.GetComponent<CharacterMovement>();
                characterMovement.PushPlayerBackObstacle();
            }
        }
    }
}
