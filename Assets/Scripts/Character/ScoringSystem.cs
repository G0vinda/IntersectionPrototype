using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class ScoringSystem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Vector3 scorePunchScale;
        [SerializeField] private float scorePunchTime;
        [SerializeField] private HighScoreNamePrompt highScoreNamePrompt;
        [SerializeField] private UIHighScoreEntryList highScoreEntryList;

        private int _currentScore;
        private CharacterAttributes.CharShape _currentShape;
        private CharacterAttributes.CharColor _currentColor;
    
        private void Start()
        {
            scoreText.text = _currentScore.ToString();
            //PlayerPrefs.DeleteKey("HighScores");
        }

        public void SetTextActive(bool value)
        {
            scoreText.gameObject.SetActive(value);
        }

        public void IncrementScore()
        {
            _currentScore++;
            UpdateScoreText();
        }
    
        public void DecrementScore()
        {
            _currentScore--;
            UpdateScoreText();
        }

        public void ChangeScore(int changeVal)
        {
            _currentScore += changeVal;
            UpdateScoreText();
        }

        public void ResetScore()
        {
            _currentScore = 0;
            scoreText.text = _currentScore.ToString();
        }

        private void UpdateScoreText()
        {
            scoreText.text = _currentScore.ToString();
            scoreText.transform.localScale = Vector3.one;
            scoreText.transform.DOPunchScale(scorePunchScale, scorePunchTime);
        }

        public void GoToHighScores(CharacterAttributes.CharShape shape, CharacterAttributes.CharColor color)
        {
            _currentShape = shape;
            _currentColor = color;
            
            highScoreNamePrompt.Initialize(_currentScore, this);
            highScoreNamePrompt.gameObject.SetActive(true);
        }

        public void ShowHighScoreList(string currentPlayerName)
        {
            var savedHighScoresString = PlayerPrefs.GetString("HighScores", "");
            var highScores = new List<HighScoreEntryData>();

            if (!string.IsNullOrEmpty(savedHighScoresString))
            {
                var savedHighScoreList = JsonUtility.FromJson<HighScoreEntryList>(savedHighScoresString);
                highScores = savedHighScoreList.highScores;
            }
        
            highScores.Add(new HighScoreEntryData(_currentScore, currentPlayerName, _currentShape, _currentColor));
            _currentScore = 0;
            highScoreEntryList.DisplayHighScores(highScores);
            var highScoresString = JsonUtility.ToJson(new HighScoreEntryList(highScores));
            PlayerPrefs.SetString("HighScores", highScoresString);
        }

        [Serializable]
        private class HighScoreEntryList
        {
            public HighScoreEntryList(List<HighScoreEntryData> highScores)
            {
                this.highScores = highScores;
            }
        
            public List<HighScoreEntryData> highScores;
        }
    
        [Serializable]
        public class HighScoreEntryData
        {
            public HighScoreEntryData(int highScore, string playerName, CharacterAttributes.CharShape shape, CharacterAttributes.CharColor color)
            {
                this.highScore = highScore;
                this.playerName = playerName;
                playerShape = (int)shape;
                playerColor = (int)color;
            }
        
            public int highScore;
            public string playerName;
            public int playerShape;
            public int playerColor;
        }
    }
}
