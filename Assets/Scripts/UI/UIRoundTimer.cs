﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIRoundTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Image timerBackground;

        private float _maxTime;

        public void EnableTimer(float maxTime)
        {
            timerText.text = Mathf.CeilToInt(maxTime).ToString();
            timerBackground.fillAmount = 1f;
            gameObject.SetActive(true);

            _maxTime = maxTime;
        }

        public void HideTimer()
        {
            gameObject.SetActive(false);
        }

        public void UpdateTimerUI(float currentTime)
        {
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
            timerBackground.fillAmount = currentTime / _maxTime;
        }
    }
}