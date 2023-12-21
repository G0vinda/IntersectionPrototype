using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Character
{
    public class CharacterAppearance : MonoBehaviour
    {
        [SerializeField] private GameObject[] shapes;
        [SerializeField] private Color[] colors;
        [SerializeField] private float invincibilityBlinkPauseTime;
        
        private WaitForSeconds _invincibilityBlinkPause;
        private Coroutine _invincibilityBlinkRoutine;
        private GameObject _currentShape;
        private Color _currentColor;
        

        public void Initialize()
        {
            _invincibilityBlinkPause = new WaitForSeconds(invincibilityBlinkPauseTime);
        }

        public int GetColorLength()
        {
            return colors.Length;
        }

        public int GetShapesLength()
        {
            return shapes.Length;
        }

        public void SetAppearance(CharacterAttributes.CharShape shape, CharacterAttributes.CharColor color)
        {
            var shapeIndex = (int)shape;
            var colorIndex = (int)color;
            SetAppearance(shapeIndex, colorIndex);
        }

        public void SetAppearance(int shapeIndex, int colorIndex)
        {
            for (var i = 0; i < shapes.Length; i++)
            {
                shapes[i].SetActive(false);
            }

            _currentShape = shapes[shapeIndex];
            _currentColor = colors[colorIndex];
            _currentShape.SetActive(true);
            _currentShape.GetComponent<MeshRenderer>().material.color = _currentColor;
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
            var activeShape = shapes.First(shape => shape.activeSelf);
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