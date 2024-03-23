using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
{
    public class LayoutUIField : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private TextMeshProUGUI layoutNameText;
        [SerializeField] private List<Image> transparencyElements;

        public static Action<int> EditButtonPressed;
        public static Action<int> DeleteButtonPressed;
        public static Action<int> CopyButtonPressed;
        public static Action<int> DragStarted;
        public static Action DragEnded;

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

        public void Copy()
        {
            CopyButtonPressed?.Invoke(_index);
        }

        public void Delete()
        {
            DeleteButtonPressed?.Invoke(_index);
        }

        public void SetTransparency(bool toTransparent)
        {
            foreach (var element in transparencyElements)
            {
                var color = element.color;
                color.a = toTransparent ? 0.6f : 1.0f;
                element.color = color;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            DragStarted?.Invoke(_index);
            SetTransparency(true);
            StartCoroutine(Dragging());
        }

        private IEnumerator Dragging()
        {
            while (true)
            {
                if(Input.GetMouseButtonUp(0))
                    break;

                yield return null;
            }

            DragEnded?.Invoke();
            SetTransparency(false);
        }
    }
}

#endif