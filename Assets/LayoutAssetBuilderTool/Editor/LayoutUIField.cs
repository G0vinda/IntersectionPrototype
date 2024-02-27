using System;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR

namespace GridCreationTool
{
    public class LayoutUIField : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI layoutName;

        public static Action<int> EditButtonPressed;
        public static Action<int> DeleteButtonPressed;

        private int _index;
            
        public void Initialize(int index)
        {
            layoutName.text = $"Layout {index}";
            _index = index;
        }

        public void UpdateIndex(int index)
        {
            _index = index;
        }

        public void Edit()
        {
            EditButtonPressed?.Invoke(_index);
        }

        public void Delete()
        {
            DeleteButtonPressed?.Invoke(_index);
        }
    }
}

#endif