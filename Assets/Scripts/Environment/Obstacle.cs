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
            if (!other.TryGetComponent<PlayerMovement>(out var characterMovement))
            {
                return;
            }

            var characterAppearance = characterMovement.GetComponent<CharacterAppearance>();

            if (!_allowedColors.Contains(characterAppearance.GetAttributes().color))
            {
                CharacterCollided?.Invoke();
                characterMovement.PushPlayerBackObstacle();
                var crashPosition = Vector3.Lerp(transform.position, other.transform.position, 0.5f);

                var particleRotation = Quaternion.LookRotation(other.transform.position - transform.position);
                Instantiate(characterMovement.wallCollisionParticlePrefab, crashPosition, particleRotation);
            }
        }
    }
}
