using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Vector3 scorePunchScale;
    [SerializeField] private float scorePunchTime;
    [SerializeField] private UIHighScoreEntryList highScoreEntryList;

    private int _currentScore;
    
    private void Start()
    {
        scoreText.text = _currentScore.ToString();
    }

    public void IncrementScore()
    {
        _currentScore++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = _currentScore.ToString();
        scoreText.transform.DOPunchScale(scorePunchScale, scorePunchTime);
    }

    public void ShowHighScoreList()
    {
        var savedHighScoresString = PlayerPrefs.GetString("HighScores", "");
        var highScores = new List<HighScoreEntry>();

        if (!string.IsNullOrEmpty(savedHighScoresString))
        {
            var savedHighScoreList = JsonUtility.FromJson<HighScoreEntryList>(savedHighScoresString);
            highScores = savedHighScoreList.highScores;
        }
        
        highScores.Add(new HighScoreEntry(_currentScore));
        highScoreEntryList.DisplayHighScores(highScores);
        var highScoresString = JsonUtility.ToJson(new HighScoreEntryList(highScores));
        PlayerPrefs.SetString("HighScores", highScoresString);
    }

    [Serializable]
    private class HighScoreEntryList
    {
        public HighScoreEntryList(List<HighScoreEntry> highScores)
        {
            this.highScores = highScores;
        }
        
        public List<HighScoreEntry> highScores;
    }
    
    [Serializable]
    public class HighScoreEntry
    {
        public HighScoreEntry(int highScore)
        {
            this.highScore = highScore;
        }
        
        public int highScore;
    }
}
