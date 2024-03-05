using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;

namespace Character
{
    public class ScoringSystem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Vector3 scorePunchScale;
        [SerializeField] private float scorePunchTime;

        public int score {get; private set;}
        public Action<int> ScoreChanged;
    
        private void Start()
        {
            scoreText.text = score.ToString();
        }

        public void SetTextActive(bool value)
        {
            scoreText.gameObject.SetActive(value);
        }

        public void IncrementScore()
        {
            ChangeScore(1);
        }
    
        public void DecrementScore()
        {
            ChangeScore(-1);
        }

        public void ChangeScore(int changeVal)
        {
            score += changeVal;
            UpdateScoreText();
            ScoreChanged?.Invoke(score);
        }

        public void ResetScore()
        {
            score = 0;
            scoreText.text = score.ToString();
        }

        private void UpdateScoreText()
        {
            scoreText.text = score.ToString();
            scoreText.transform.localScale = Vector3.one;
            scoreText.transform.DOPunchScale(scorePunchScale, scorePunchTime);
        }
    
        public class HighScoreEntryData
        {
            public HighScoreEntryData(int score, string playerName, CharacterAttributes characterAttributes)
            {
                this.score = score;
                this.playerName = playerName;
                this.characterAttributes = characterAttributes;
            }

            public override string ToString()
            {
                var serializable = new SerializableHighScoreEntryData(this);
                return JsonUtility.ToJson(serializable);
            }

            public static HighScoreEntryData FromString(string serialzedString)
            {
                var serializableData = JsonUtility.FromJson<SerializableHighScoreEntryData>(serialzedString);
                return new HighScoreEntryData(
                    serializableData.score, 
                    serializableData.playerName, 
                    CharacterAttributes.FromString(serializableData.serializedCharacterAttributes));
            }

            public int score;
            public string playerName;
            public CharacterAttributes characterAttributes;

            private class SerializableHighScoreEntryData
            {
                public int score;
                public string playerName;
                public string serializedCharacterAttributes;

                public SerializableHighScoreEntryData(HighScoreEntryData highScoreEntryData)
                {
                    score = highScoreEntryData.score;
                    playerName = highScoreEntryData.playerName;
                    serializedCharacterAttributes = highScoreEntryData.characterAttributes.ToString();
                }
            }
        }
    }
}
