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

        public enum CharPattern
        {
            Check,
            Lined,
            None
        }

        public struct SpawnRestrictions
        {
            // set values to -1 if you don't want to set a restriction
            public CharShape shapeIndex; 
            public CharColor colorIndex;

            public static SpawnRestrictions none => new SpawnRestrictions((CharShape)(-1), (CharColor)(-1));

            public SpawnRestrictions(CharShape shape, CharColor color)
            {
                this.shapeIndex = shape;
                this.colorIndex = color;
            }
        }
    }
}