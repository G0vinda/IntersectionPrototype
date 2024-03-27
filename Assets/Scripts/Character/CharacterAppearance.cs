using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Character
{
    public class CharacterAppearance : MonoBehaviour
    {
        [SerializeField] private CharacterShape[] shapes;
        [SerializeField] private Color[] colors;
        [SerializeField] private Material[] materials;
        [SerializeField] private Material npcMaterial;
        [SerializeField] private float invincibilityBlinkPauseTime;
        
        private WaitForSeconds _invincibilityBlinkPause;
        private Coroutine _invincibilityBlinkRoutine;
        private CharacterShape _currentShape;
        private CharacterAttributes _attributes;        

        void Awake()
        {
            _invincibilityBlinkPause = new WaitForSeconds(invincibilityBlinkPauseTime);
        }

        public void SetAttributes(CharacterAttributes attributes)
        {
            _attributes = attributes;
            var shapeIndex = (int)attributes.shape;
            var colorIndex = (int)attributes.color;
            var patternIndex = (int)attributes.pattern;
            SetAppearance(shapeIndex, colorIndex, patternIndex);
        }

        public CharacterAttributes GetAttributes()
        {
            return _attributes;
        }

        private void SetAppearance(int shapeIndex, int colorIndex, int patternIndex)
        {
            var isNpc = TryGetComponent<NpcMovement>(out _);
            if(isNpc)
                patternIndex = 2;

            for (var i = 0; i < shapes.Length; i++)
            {
                shapes[i].gameObject.SetActive(false);
            }

            _currentShape = shapes[shapeIndex];
            _currentShape.SetPattern((CharacterAttributes.Pattern)patternIndex);
            _currentShape.gameObject.SetActive(true);
            _currentShape.GetComponent<MeshRenderer>().material = isNpc ? npcMaterial : materials[colorIndex];
        }

        public void StartInvincibilityBlinking()
        {
            _invincibilityBlinkRoutine = StartCoroutine(InvincibilityBlinking());
        }

        public void StopInvincibilityBlinking()
        {
            StopCoroutine(_invincibilityBlinkRoutine);
            var currentMaterial = _currentShape.GetComponent<MeshRenderer>().material;
            currentMaterial.SetFloat("_Alpha", 1.0f);
        }

        private IEnumerator InvincibilityBlinking()
        {
            var activeShape = shapes.First(shape => shape.gameObject.activeSelf);
            var activeMaterial = activeShape.GetComponent<MeshRenderer>().material;

            do
            {
                activeMaterial.SetFloat("_Alpha", 0.4f);
                yield return _invincibilityBlinkPause;

                activeMaterial.SetFloat("_Alpha", 0.9f);
                yield return _invincibilityBlinkPause;
            } while (true);
        }
    }
}