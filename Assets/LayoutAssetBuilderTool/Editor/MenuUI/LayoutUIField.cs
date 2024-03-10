using System;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
{
    public class LayoutUIField : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI layoutNameText;

        public static Action<int> EditButtonPressed;
        public static Action<int> DeleteButtonPressed;

        private int _index;
            
        public void Initialize(int index, string name)
        {
            _index = index;
            layoutNameText.text = name;
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