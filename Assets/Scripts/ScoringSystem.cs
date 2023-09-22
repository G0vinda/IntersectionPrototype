using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Vector3 scorePunchScale;
    [SerializeField] private float scorePunchTime;

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
}
