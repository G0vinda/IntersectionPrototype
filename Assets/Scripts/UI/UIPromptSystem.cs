using System;
using UnityEngine;

namespace UI
{
    public class UIPromptSystem : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;

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

        private void EnableInput(Type _)
        {
            inputManager.enabled = true;
        }
    }
}
