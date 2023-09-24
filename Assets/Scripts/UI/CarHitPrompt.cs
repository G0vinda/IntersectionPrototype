using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CarHitPrompt : UIPrompt
    {
        [SerializeField] private Image timerImage;

        private float _maxTime;

        public void SetTimer(float time)
        {
            _maxTime = time;
            StartCoroutine(ProcessTimer());
        }

        private IEnumerator ProcessTimer()
        {
            var timer = _maxTime;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                timerImage.fillAmount = timer / _maxTime;
                
                yield return null;
            }
            
            gameObject.SetActive(false);
            TriggerPromptClosed(typeof(CarHitPrompt));
        }
    }
}
