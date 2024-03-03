using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Character
{
    [Serializable]
    public class CharacterAttributes
    {
        public CharShape shape { get; private set; }
        public CharColor color { get; private set; }
        public CharPattern pattern { get; private set; }

        public CharacterAttributes(CharShape shape, CharColor color, CharPattern pattern)
        {
            this.shape = shape;
            this.color = color;
            this.pattern = pattern;
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

        public static CharacterAttributes FromString(string serializedAttributes)
        {
            var attributeStrings = serializedAttributes.Split(',');
            if(!Enum.TryParse(attributeStrings[0], out CharShape shape) ||
                !Enum.TryParse(attributeStrings[1], out CharColor color) ||
                !Enum.TryParse(attributeStrings[2], out CharPattern pattern))
            {
                throw new Exception("Error on Deserializing CharacterAttribute string");
            }
            
            return new CharacterAttributes(shape, color, pattern);
        }

        public override string ToString()
        {
            return shape.ToString() + "," + color.ToString() + "," + pattern.ToString();
        }

        public static CharacterAttributes GetRandomAttributes(SpawnRestrictions spawnRestrictions, CharacterAttributes excludeAttributes = null)
        {
            int excludedShapeIndex;
            int excludedColorIndex;
            int excludedPatternIndex;

            if(excludeAttributes != null)
            {
                excludedShapeIndex = (int)spawnRestrictions.shape == -1 ? (int)excludeAttributes.shape : -1;
                excludedColorIndex = (int)spawnRestrictions.color == -1 ? (int)excludeAttributes.color : -1;
                excludedPatternIndex = (int)spawnRestrictions.pattern == -1 ? (int)excludeAttributes.pattern : -1;
            }
            else
            {
                excludedShapeIndex = excludedColorIndex = excludedPatternIndex = -1;
            }

            CharacterAttributes randomAttributes;
            do
            {
                randomAttributes = new CharacterAttributes(
                    (int)spawnRestrictions.shape == -1 ? (CharShape)Random.Range(0, GetShapesLength()) : spawnRestrictions.shape,
                    (int)spawnRestrictions.color == -1 ? (CharColor)Random.Range(0, GetColorsLength()) : spawnRestrictions.color,
                    (int)spawnRestrictions.pattern == -1 ? (CharPattern)Random.Range(0, GetPatternsLength()) : spawnRestrictions.pattern
                );
            } while((int)randomAttributes.shape == excludedShapeIndex || (int)randomAttributes.color == excludedColorIndex || (int)randomAttributes.pattern == excludedPatternIndex);

            return randomAttributes;
        }
        
        [Serializable]
        public enum CharColor
        {
            Blue,
            Red,
            Yellow
        }

        [Serializable]
        public enum CharShape
        {
            Cube,
            Pyramid,
            Sphere
        }

        [Serializable]
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