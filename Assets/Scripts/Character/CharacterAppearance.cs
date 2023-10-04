using System;
using UnityEngine;

namespace Character
{
    public class CharacterAppearance : MonoBehaviour
    {
        [SerializeField] private GameObject[] shapes;
        [SerializeField] private Color[] colors;

        private Material[] _materials;

        public void Initialize()
        {
            _materials = new Material[colors.Length];
            for (var i = 0; i < shapes.Length; i++)
            {
                _materials[i] = shapes[i].GetComponent<MeshRenderer>().material;
            }
        }

        public int GetColorLength()
        {
            return colors.Length;
        }

        public int GetShapesLength()
        {
            return shapes.Length;
        }

        public void SetAppearance(int shapeIndex, int colorIndex)
        {
            for (var i = 0; i < shapes.Length; i++)
            {
                shapes[i].SetActive(i == shapeIndex);
                _materials[i].color = colors[colorIndex];
            }
        }
    }
}