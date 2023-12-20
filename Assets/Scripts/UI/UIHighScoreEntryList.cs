using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Character;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class UIHighScoreEntryList : MonoBehaviour
    {
        [SerializeField] private GameObject uiParentElement;
        [SerializeField] private UIHighScoreEntry highScoreEntryPrefab;
        [SerializeField] private ScrollRect scrollView;

        private VerticalLayoutGroup _layoutGroup;

        private float _highScoreEntryHeight;
        private float _highScoreEntrySpacing;
        private float _listContentHeight;
        private float _scrollViewHeight;

        private void SetupSizeValues()
        {
            _layoutGroup = GetComponent<VerticalLayoutGroup>();
            _highScoreEntrySpacing = _layoutGroup.spacing;

            _highScoreEntryHeight = highScoreEntryPrefab.GetComponent<RectTransform>().sizeDelta.y;
            
            _scrollViewHeight = scrollView.GetComponent<RectTransform>().sizeDelta.y;
        }

        public void DisplayHighScores(List<ScoringSystem.HighScoreEntryData> highScoreEntries)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);    
            }
            
            uiParentElement.SetActive(true);
            var currentEntry = highScoreEntries.Last();
            var sortedHighScores = highScoreEntries.OrderByDescending(entry => entry.highScore).ToList();
            var currentEntryId = 0;
            for (var i = 0; i < sortedHighScores.Count; i++)
            {
                var shouldBeMarked = false;
                if (currentEntry == sortedHighScores[i])
                {
                    shouldBeMarked = true;
                    currentEntryId = i;
                }
                
                var newEntry = Instantiate(highScoreEntryPrefab, transform);
                newEntry.Initialize(sortedHighScores[i], i + 1, shouldBeMarked);
            }
            
            if(_layoutGroup == null)
                SetupSizeValues();
            
            StartCoroutine(AlignScrollViewToEntry(currentEntryId));
        }

        private IEnumerator AlignScrollViewToEntry(int entryId)
        {
            yield return null;
            
            _listContentHeight = _layoutGroup.GetComponent<RectTransform>().sizeDelta.y;
            var entryYPositionFromTop = (entryId + 1) * _highScoreEntryHeight + entryId * _highScoreEntrySpacing;

            if (entryYPositionFromTop > _scrollViewHeight)
            {
                var entryYPositionFromBottom = _listContentHeight - entryYPositionFromTop;
                var scrollPosition = entryYPositionFromBottom / (_listContentHeight - _scrollViewHeight);
                scrollView.verticalNormalizedPosition = scrollPosition;
            }
        }
    }
}