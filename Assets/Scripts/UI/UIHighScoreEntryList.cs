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
        [SerializeField] private GameObject loadingIndicator;

        private VerticalLayoutGroup _layoutGroup;

        private float _highScoreEntryHeight;
        private float _highScoreEntrySpacing;
        private float _listContentHeight;
        private float _scrollViewHeight;
        private bool _isLoading;
        private ScoringSystem.HighScoreEntryData _highScoreEntryDataToBeMarked;

        public void Initialize(ScoringSystem.HighScoreEntryData highScoreEntryDataToBeMarked)
        {
            _highScoreEntryDataToBeMarked = highScoreEntryDataToBeMarked;
            _isLoading = true;
            LootLockerManager.Instance.highScoreListFetched += DisplayHighScores;
            LootLockerManager.Instance.FetchHighScoreList();
        }

        private void DisplayHighScores(List<ScoringSystem.HighScoreEntryData> highScoreEntries)
        {
            _isLoading = false;
            LootLockerManager.Instance.highScoreListFetched -= DisplayHighScores;
            Destroy(loadingIndicator);

            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);    
            }
            
            uiParentElement.SetActive(true);
            var currentEntryId = 0;
            for (var i = 0; i < highScoreEntries.Count; i++)
            {
                var shouldBeMarked = false;
                if (_highScoreEntryDataToBeMarked != null && _highScoreEntryDataToBeMarked.Equals(highScoreEntries[i]))
                {
                    shouldBeMarked = true; 
                    currentEntryId = i;
                }
                
                var newEntry = Instantiate(highScoreEntryPrefab, transform);
                newEntry.Initialize(highScoreEntries[i], i + 1, shouldBeMarked);
            }
            
            if(_layoutGroup == null)
                SetupSizeValues();
            
            StartCoroutine(AlignScrollViewToEntry(currentEntryId));
        }

        public void TryAgainClicked()
        {
            if(_isLoading)
                LootLockerManager.Instance.highScoreListFetched -= DisplayHighScores;

            FlowManager.Instance.ContinueClicked();
        }

        public void BackClicked()
        {
            if(_isLoading)
                LootLockerManager.Instance.highScoreListFetched -= DisplayHighScores;

            FlowManager.Instance.GoBackToTitleMenu();
        }

        private void SetupSizeValues()
        {
            _layoutGroup = GetComponent<VerticalLayoutGroup>();
            _highScoreEntrySpacing = _layoutGroup.spacing;

            _highScoreEntryHeight = highScoreEntryPrefab.GetComponent<RectTransform>().sizeDelta.y;
            
            _scrollViewHeight = scrollView.GetComponent<RectTransform>().sizeDelta.y;
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