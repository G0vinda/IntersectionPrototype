using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LayoutUIName : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private UnityEvent ClickForward;

        public Action GotDoubleClicked;
        public string text {
            get => _nameText.text;
            set => _nameText.text = value;
        }
        
        private readonly float _doubleClickTime = 0.5f;
        private bool _justGotClicked;
        private TextMeshProUGUI _nameText;

        void Awake()
        {
            _nameText = GetComponent<TextMeshProUGUI>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ClickForward?.Invoke();

            if(_justGotClicked)
                StartCoroutine(ProcessSecondClick());
            else
                StartCoroutine(ProcessFirstClick());
        }

        private IEnumerator ProcessFirstClick()
        {
            _justGotClicked = true;
            yield return new WaitForSeconds(_doubleClickTime);
            _justGotClicked = false;
        }

        private IEnumerator ProcessSecondClick()
        {
            while (_justGotClicked)
            {
                if(Input.GetMouseButtonUp(0))    
                {
                    GotDoubleClicked?.Invoke();
                    _justGotClicked = false;
                }

                yield return null;
            }
        }
    }
}

#endif