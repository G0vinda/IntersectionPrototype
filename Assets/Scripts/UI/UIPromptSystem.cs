using System;
using UnityEngine;

namespace UI
{
    public class UIPromptSystem : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [Header("Car hit prompt")]
        [SerializeField] private CarHitPrompt carHitPrompt;
        [SerializeField] private float carHitPromptTime;

        public static UIPromptSystem Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        #region OnEnable/OnDisable

        private void OnEnable()
        {
            UIPrompt.PromptClosed += EnableInput;
        }

        private void OnDisable()
        {
            UIPrompt.PromptClosed -= EnableInput;
        }

        #endregion

        public void ShowCarHitPrompt()
        {
            carHitPrompt.gameObject.SetActive(true);
            carHitPrompt.SetTimer(carHitPromptTime);
            inputManager.enabled = false;
        }

        private void EnableInput(Type _)
        {
            inputManager.enabled = true;
        }
    }
}
