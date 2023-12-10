using System;
using UnityEngine;

namespace Character
{
    public class CharacterAttributes : MonoBehaviour
    {
        private CharColor _color;
        private CharShape _shape;

        public void SetAttributes(CharShape shape, CharColor color)
        {
            _color = color;
            _shape = shape;

            var characterAppearance = GetComponent<CharacterAppearance>();
            characterAppearance.Initialize();
            characterAppearance.SetAppearance(_shape, _color);
        }

        public CharColor GetColor()
        {
            return _color;
        }

        public CharShape GetShape()
        {
            return _shape;
        }
        
        public enum CharColor
        {
            Blue,
            Red,
            Yellow
        }

        public enum CharShape
        {
            Cube,
            Pyramid,
            Sphere
        }
    }
}