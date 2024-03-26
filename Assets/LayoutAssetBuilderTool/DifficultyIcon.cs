using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LayoutAssetBuilderTool
{
    public class DifficultyIcon : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] TextMeshProUGUI numText;
        [SerializeField] Color[] difficultyColors;
        [SerializeField] UnityEvent<int> OnEdit;

        private readonly float _doubleClickTime = 0.5f;
        private bool _justGotClicked;
        private Image _backgroundImage;

        void Awake()
        {
            _backgroundImage = GetComponent<Image>();
        }

        public void SetDifficulty(int difficulty)
        {
            numText.text = difficulty.ToString();
            _backgroundImage.color = GetDifficultyColor(difficulty);
        }

        public Color GetDifficultyColor(int difficulty)
        {
            return difficultyColors[difficulty];
        }

        public void OnPointerDown(PointerEventData eventData)
        {
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
                    StartCoroutine(EditMode());
                    _justGotClicked = false;
                    break;
                }

                yield return null;
            }
        }

        private IEnumerator EditMode()
        {
            transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
            var previousText = numText.text;
            numText.text = "?";

            while (true)
            {
                if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    numText.text = previousText;
                    break;
                }
                
                var pressedNumber = GetNumberKeyDown();
                if(pressedNumber >= 0)
                {
                    numText.text = pressedNumber.ToString();
                    OnEdit?.Invoke(pressedNumber);
                    break;
                }

                yield return null;
            }
        }

        private int GetNumberKeyDown()
        {
            if(Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
                return 0;
            if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                return 1;
            if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                return 2;
            if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                return 3;
            if(Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                return 4;
            if(Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                return 5;
            if(Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                return 6;
            if(Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
                return 7;
            if(Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
                return 8;
            if(Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
                return 9;

            return -1;
        }
    }
}
