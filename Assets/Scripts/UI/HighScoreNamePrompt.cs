using System.Data.Common;
using Character;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class HighScoreNamePrompt : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreInfoText;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button cancelButton;

        private ScoringSystem.HighScoreEntryData _scoreData;
        private CityLevel _cityLevel;

        public void Initialize(CityLevel cityLevel, ScoringSystem.HighScoreEntryData scoreData)
        {
            _cityLevel = cityLevel;
            scoreInfoText.text = $"You made {scoreData.score} points!";
            _scoreData = scoreData;
        }

        public void InputTextUpdated()
        {
            saveButton.interactable = inputField.text.Length > 0;
        }

        public void Save()
        {
            _scoreData.playerName = inputField.text;
            LootLockerManager.Instance.scoreGotUploaded += OnScoreUploadSucceeded;
            LootLockerManager.Instance.UpdloadScoreToLeaderboard(_scoreData);

            saveButton.interactable = false;
            cancelButton.interactable = false;
            inputField.interactable = false;
        }

        public void CancelClicked()
        {
            FlowManager.Instance.BackClicked();
        }

        private void OnScoreUploadSucceeded()
        {
            LootLockerManager.Instance.scoreGotUploaded -= OnScoreUploadSucceeded;
            _cityLevel.ShowHighScoreList(_scoreData);
            gameObject.SetActive(false);
        }
    }
}