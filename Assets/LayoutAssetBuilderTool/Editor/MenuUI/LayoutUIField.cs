using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
{
    public class LayoutUIField : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private LayoutUIName layoutName;
        [SerializeField] private LayoutUINameEdit layoutNameEdit;
        [SerializeField] private DifficultyIcon difficultyIcon;
        [SerializeField] private Image background;
        [SerializeField] private List<Image> transparencyElements;

        public static Action<int> EditButtonPressed;
        public static Action<int> DeleteButtonPressed;
        public static Action<int> CopyButtonPressed;
        public static Action<int> DragStarted;
        public static Action<int, string> NameChanged;
        public static Action<int, int> DifficultyChanged;
        public static Action DragEnded;

        private int _index;

        #region OnEnable/OnDisable

        void OnEnable()
        {
            layoutName.GotDoubleClicked += StartNameEdit;
            layoutNameEdit.NameEditSubmitted += NameEditSubmitted;
            layoutNameEdit.NameEditCanceled += NameEditCanceled;
        }

        void OnDisable()
        {
            layoutName.GotDoubleClicked -= StartNameEdit;
            layoutNameEdit.NameEditSubmitted -= NameEditSubmitted;
            layoutNameEdit.NameEditCanceled -= NameEditCanceled;
        }

        #endregion
            
        public void Initialize(int index, string name, int difficulty)
        {
            _index = index;
            layoutName.text = name;
            difficultyIcon.SetDifficulty(difficulty);
            
            background.color = CalculateBackgroundColor(difficultyIcon.GetDifficultyColor(difficulty));
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

        private void StartNameEdit()
        {
            layoutName.gameObject.SetActive(false);
            layoutNameEdit.gameObject.SetActive(true);
            layoutNameEdit.StartEdit(layoutName.text);
        }

        private void NameEditSubmitted(string newName)
        {
            layoutName.text = newName;
            NameChanged?.Invoke(_index, newName);
            layoutNameEdit.gameObject.SetActive(false);
            layoutName.gameObject.SetActive(true);
        }

        private void NameEditCanceled()
        {
            layoutNameEdit.gameObject.SetActive(false);
            layoutName.gameObject.SetActive(true);
        }

        public void OnDifficultyChanged(int newDifficulty)
        {
            DifficultyChanged?.Invoke(_index, newDifficulty);
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
            OnClick();
        }

        public void OnClick()
        {
            DragStarted?.Invoke(_index);
            StartCoroutine(Dragging());
        }

        private IEnumerator Dragging()
        {
            var timeToBecomeTransparent = 0.15f;
            while (true)
            {
                if(Input.GetMouseButtonUp(0))
                    break;
                
                if(timeToBecomeTransparent > 0)
                {
                    timeToBecomeTransparent -= Time.deltaTime;
                    if(timeToBecomeTransparent <= 0)
                        SetTransparency(true);
                }

                yield return null;
            }

            DragEnded?.Invoke();
            SetTransparency(false);
        }

        private Color CalculateBackgroundColor(Color color)
        {
            Color.RGBToHSV(color, out var H, out var S, out var V);
            S -= S*0.3f;
            V += V*0.2f;

            return Color.HSVToRGB(H, S, V);
        }
    }
}

#endif