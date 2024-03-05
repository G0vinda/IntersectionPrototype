using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIRoundTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Image timerBackground;

        public float maxTime;

        public void SetTextActive()
        {
            timerText.gameObject.SetActive(true);
        }

        public void UpdateTimerUI(float currentTime)
        {
            timerText.text = currentTime.ToString("n1", CultureInfo.CreateSpecificCulture("en-GB"));
            timerBackground.fillAmount = currentTime / maxTime;
        }
    }
}