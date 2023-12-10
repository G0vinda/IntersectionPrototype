using System.Collections.Generic;
using System.Linq;
using Character;
using UnityEngine;

namespace UI
{
    public class UIHighScoreEntryList : MonoBehaviour
    {
        [SerializeField] private GameObject uiParentElement;
        [SerializeField] private UIHighScoreEntry highScoreEntryPrefab;

        public void DisplayHighScores(List<ScoringSystem.HighScoreEntryData> highScoreEntries)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);    
            }
            
            uiParentElement.SetActive(true);
            var currentEntry = highScoreEntries.Last();
            var sortedHighScores = highScoreEntries.OrderByDescending(entry => entry.highScore).ToList();
            for (var i = 0; i < sortedHighScores.Count; i++)
            {
                var shouldBeMarked = currentEntry == sortedHighScores[i];
                var newEntry = Instantiate(highScoreEntryPrefab, transform);
                newEntry.Initialize(sortedHighScores[i], i + 1, shouldBeMarked);
            }
        }
    }
}