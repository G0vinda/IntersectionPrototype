using System;
using UnityEngine;

namespace UI
{
    public class UIPrompt : MonoBehaviour
    {
        public static event Action<Type> PromptClosed;

        protected void TriggerPromptClosed(Type promptType)
        {
            Debug.Log($"Prompt of type {promptType} was closed.");
            PromptClosed?.Invoke(promptType);
        }
    }
}