using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Character
{
    [Serializable]
    public class CharacterAttributes
    {
        public Shape shape { get; private set; }
        public Color color { get; private set; }
        public Pattern pattern { get; private set; }

        public CharacterAttributes(Shape shape, Color color, Pattern pattern)
        {
            this.shape = shape;
            this.color = color;
            this.pattern = pattern;
        }

        public static int GetColorsLength()
        {
            return Enum.GetNames(typeof(Color)).Length;
        }

        public static int GetShapesLength()
        {
            return Enum.GetNames(typeof(Shape)).Length;
        }

        public static int GetPatternsLength()
        {
            return Enum.GetNames(typeof(Pattern)).Length;
        }

        public static CharacterAttributes FromString(string serializedAttributes)
        {
            var attributeStrings = serializedAttributes.Split(',');
            if(!Enum.TryParse(attributeStrings[0], out Shape shape) ||
                !Enum.TryParse(attributeStrings[1], out Color color) ||
                !Enum.TryParse(attributeStrings[2], out Pattern pattern))
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
                    (int)spawnRestrictions.shape == -1 ? (Shape)Random.Range(0, GetShapesLength()) : spawnRestrictions.shape,
                    (int)spawnRestrictions.color == -1 ? (Color)Random.Range(0, GetColorsLength()) : spawnRestrictions.color,
                    (int)spawnRestrictions.pattern == -1 ? (Pattern)Random.Range(0, GetPatternsLength()) : spawnRestrictions.pattern
                );
            } while((int)randomAttributes.shape == excludedShapeIndex || (int)randomAttributes.color == excludedColorIndex || (int)randomAttributes.pattern == excludedPatternIndex);

            return randomAttributes;
        }
        
        [Serializable]
        public enum Color
        {
            Blue,
            Red,
            Yellow
        }

        [Serializable]
        public enum Shape
        {
            Cube,
            Pyramid,
            Sphere
        }

        [Serializable]
        public enum Pattern
        {
            Check,
            Lined,
            None
        }

        public struct SpawnRestrictions
        {
            // set values to -1 if you don't want to set a restriction
            public Shape shape; 
            public Color color;
            public Pattern pattern;

            public static SpawnRestrictions none => new SpawnRestrictions((Shape)(-1), (Color)(-1), (Pattern)(-1));

            public SpawnRestrictions(Shape shape, Color color, Pattern pattern)
            {
                this.shape = shape;
                this.color = color;
                this.pattern = pattern;
            }
        }
    }
}