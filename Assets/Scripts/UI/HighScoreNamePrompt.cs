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
        [SerializeField] private UnityEvent cancelEvent;

        private ScoringSystem _scoringSystem;

        public void Initialize(int score, ScoringSystem scoringSystem)
        {
            scoreInfoText.text = $"You made {score} points!";
            var lastPlayerName = PlayerPrefs.GetString("lastPlayerName", "");
            if(lastPlayerName.Length > 0 )
                inputField.text = lastPlayerName;
            
            gameObject.SetActive(false);
            _scoringSystem = scoringSystem;
        }

        public void InputTextUpdated()
        {
            saveButton.interactable = inputField.text.Length > 0;
        }

        public void Save()
        {
            _scoringSystem.ShowHighScoreList(inputField.text);
            PlayerPrefs.SetString("lastPlayerName", inputField.text);
            
            gameObject.SetActive(false);
        }

        public void Cancel()
        {
            cancelEvent.Invoke();
        }
    }
}