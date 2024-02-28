using System;
using System.Collections;
using System.Linq;
using log4net.Util;
using UnityEngine;

namespace Character
{
    public class CharacterAppearance : MonoBehaviour
    {
        [SerializeField] private CharacterShape[] shapes;
        [SerializeField] private Color[] colors;
        [SerializeField] private float invincibilityBlinkPauseTime;
        
        private WaitForSeconds _invincibilityBlinkPause;
        private Coroutine _invincibilityBlinkRoutine;
        private CharacterShape _currentShape;
        private Color _currentColor;
        

        public void Initialize()
        {
            _invincibilityBlinkPause = new WaitForSeconds(invincibilityBlinkPauseTime);
        }

        public void SetAppearance(CharacterAttributes.CharShape shape, CharacterAttributes.CharColor color, CharacterAttributes.CharPattern pattern)
        {
            var shapeIndex = (int)shape;
            var colorIndex = (int)color;
            var patternIndex = (int)pattern;
            SetAppearance(shapeIndex, colorIndex, patternIndex);
        }

        public void SetAppearance(int shapeIndex, int colorIndex, int patternIndex)
        {
            for (var i = 0; i < shapes.Length; i++)
            {
                shapes[i].gameObject.SetActive(false);
            }

            _currentShape = shapes[shapeIndex];
            _currentColor = colors[colorIndex];
            _currentShape.SetPattern((CharacterAttributes.CharPattern)patternIndex);
            _currentShape.gameObject.SetActive(true);
            _currentShape.GetComponent<MeshRenderer>().material.SetColor("_Color_Outline", _currentColor);
        }

        public void StartInvincibilityBlinking()
        {
            _invincibilityBlinkRoutine = StartCoroutine(InvincibilityBlinking());
        }

        public void StopInvincibilityBlinking()
        {
            StopCoroutine(_invincibilityBlinkRoutine);
            _currentShape.GetComponent<MeshRenderer>().material.color = _currentColor;
        }

        private IEnumerator InvincibilityBlinking()
        {
            var activeShape = shapes.First(shape => shape.gameObject.activeSelf);
            var activeMaterial = activeShape.GetComponent<MeshRenderer>().material;
            var color1 = activeMaterial.color;
            var color2 = activeMaterial.color;
            color1.a = 0.8f;
            color2.a = 0.4f;

            do
            {
                activeMaterial.color = color1;
                yield return _invincibilityBlinkPause;

                activeMaterial.color = color2;
                yield return _invincibilityBlinkPause;
            } while (true);
        }
    }
}