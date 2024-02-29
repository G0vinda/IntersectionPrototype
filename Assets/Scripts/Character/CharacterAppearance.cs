using System.Collections;
using System.Linq;
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
            _currentShape.SetPattern((CharacterAttributes.CharPattern)patternIndex);
            _currentShape.gameObject.SetActive(true);
            var currentMaterial = _currentShape.GetComponent<MeshRenderer>().material;
            currentMaterial.SetColor("_Color_Outline", colors[colorIndex]);
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

                activeMaterial.SetFloat("_Alpha", 0.8f);
                yield return _invincibilityBlinkPause;
            } while (true);
        }
    }
}