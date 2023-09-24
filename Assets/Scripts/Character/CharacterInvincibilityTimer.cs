using System;
using System.Collections;
using UI;
using UnityEngine;

namespace Character
{
    public class CharacterInvincibilityTimer : MonoBehaviour
    {
        [SerializeField] private float characterInvincibilityTime;
        [SerializeField] private float blinkTime;
        [SerializeField] private float blinkTransparencyValue;

        private Collider _collider;
        private MeshRenderer _meshRenderer;
        private bool _meshIsTransparent;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
        }

        #region OnEnable/OnDisable

        private void OnEnable()
        {
            UIPrompt.PromptClosed += StartTimerIfCarHitPromptClosed;
        }

        private void OnDisable()
        {
            UIPrompt.PromptClosed -= StartTimerIfCarHitPromptClosed;
        }

        #endregion

        private void StartTimerIfCarHitPromptClosed(Type promptType)
        {
            if(promptType != typeof(CarHitPrompt))
                return;
            
            StartTimer();
        }

        public void StartTimer()
        {
            StartCoroutine(InvincibilityTimer());
        }
        
        private IEnumerator InvincibilityTimer()
        {
            var timer = characterInvincibilityTime;
            var blinkTimer = blinkTime;
            var meshColor = _meshRenderer.material.color;
            _collider.enabled = false;
            
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                blinkTimer -= Time.deltaTime;
                if (blinkTimer < 0)
                {
                    blinkTimer = blinkTime;
                    meshColor.a = _meshIsTransparent ? 1f : blinkTransparencyValue;
                    _meshIsTransparent = !_meshIsTransparent;
                    _meshRenderer.material.color = meshColor;
                }
                yield return null;
            }

            _collider.enabled = true;
            meshColor.a = 1f;
            _meshRenderer.material.color = meshColor;
        }
    }
}
