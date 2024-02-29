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
        private Color _currentColorOutline;
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
            _currentColorOutline = colors[colorIndex];
            _currentShape.SetPattern((CharacterAttributes.CharPattern)patternIndex);
            _currentShape.gameObject.SetActive(true);
            var currentMaterial = _currentShape.GetComponent<MeshRenderer>().material;
            currentMaterial.SetColor("_Color_Outline", _currentColorOutline);
            _currentColor = currentMaterial.GetColor("_Color");
        }

        public void StartInvincibilityBlinking()
        {
            _invincibilityBlinkRoutine = StartCoroutine(InvincibilityBlinking());
        }

        public void StopInvincibilityBlinking()
        {
            StopCoroutine(_invincibilityBlinkRoutine);
            var currentMaterial = _currentShape.GetComponent<MeshRenderer>().material;
            currentMaterial.SetColor("_Color", _currentColor);
            currentMaterial.SetColor("_Color_Outline", _currentColorOutline);
        }

        private IEnumerator InvincibilityBlinking()
        {
            var activeShape = shapes.First(shape => shape.gameObject.activeSelf);
            var activeMaterial = activeShape.GetComponent<MeshRenderer>().material;
            Color color1;
            Color color2 = color1 = activeMaterial.GetColor("_Color");
            Color colorOutline1;
            Color colorOutline2 = colorOutline1 = activeMaterial.GetColor("_Color_Outline");
            color1.a = 0.8f;
            colorOutline1.a = 0.8f;
            color2.a = 0.4f;
            colorOutline2.a = 0.4f;

            do
            {
                activeMaterial.SetColor("_Color", color1);
                activeMaterial.SetColor("_Color_Outline", colorOutline1);
                yield return _invincibilityBlinkPause;

                activeMaterial.SetColor("_Color", color2);
                activeMaterial.SetColor("_Color_Outline", colorOutline2);
                yield return _invincibilityBlinkPause;
            } while (true);
        }
    }
}