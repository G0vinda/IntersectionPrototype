using System;
using UnityEngine;

namespace Character
{
    public class CharacterAttributes : MonoBehaviour
    {
        private CharColor _color;
        private CharShape _shape;
        private CharPattern _pattern;

        public void SetAttributes(CharShape shape, CharColor color, CharPattern pattern)
        {
            _color = color;
            _shape = shape;
            _pattern = pattern;

            var characterAppearance = GetComponent<CharacterAppearance>();
            characterAppearance.Initialize();
            characterAppearance.SetAppearance(_shape, _color, _pattern);
        }

        public CharColor GetColor()
        {
            return _color;
        }

        public CharShape GetShape()
        {
            return _shape;
        }

        public static int GetColorsLength()
        {
            return Enum.GetNames(typeof(CharColor)).Length;
        }

        public static int GetShapesLength()
        {
            return Enum.GetNames(typeof(CharShape)).Length;
        }

        public static int GetPatternsLength()
        {
            return Enum.GetNames(typeof(CharPattern)).Length;
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
            public CharShape shape; 
            public CharColor color;
            public CharPattern pattern;

            public static SpawnRestrictions none => new SpawnRestrictions((CharShape)(-1), (CharColor)(-1), (CharPattern)(-1));

            public SpawnRestrictions(CharShape shape, CharColor color, CharPattern pattern)
            {
                this.shape = shape;
                this.color = color;
                this.pattern = pattern;
            }
        }
    }
}