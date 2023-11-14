using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{
    public class UIHighScoreEntryList : MonoBehaviour
    {
        [SerializeField] private UIHighScoreEntry highScoreEntryPrefab;

        public void DisplayHighScores(List<ScoringSystem.HighScoreEntry> highScoreEntries)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);    
            }
            
            transform.parent.gameObject.SetActive(true);
            var currentEntry = highScoreEntries.Last();
            var sortedHighScores = highScoreEntries.OrderByDescending(entry => entry.highScore).ToList();
            foreach (var entry in sortedHighScores)
            {
                var shouldBeMarked = currentEntry == entry;
                var newEntry = Instantiate(highScoreEntryPrefab, transform);
                newEntry.Initialize(entry.highScore, shouldBeMarked);
            }
        }
    }
}