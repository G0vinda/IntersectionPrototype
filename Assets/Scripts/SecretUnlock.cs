using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SecretUnlock : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] int tapsNeededToUnlock;
    [SerializeField] float allowedTimeBetweenTaps;
    [SerializeField] StoryDataStorage storyDataStorage;

    private bool _isActiveCounting;
    private float _timer;
    private int _countedTaps;

    public void OnPointerDown(PointerEventData eventData)
    {
        _countedTaps ++;
        if(_countedTaps >= tapsNeededToUnlock)
            UnlockGame();

        if(!_isActiveCounting)
        {
            StartCoroutine(CountTaps());
        }
        else
        {
            _timer = allowedTimeBetweenTaps;
        }
    }

    private IEnumerator CountTaps()
    {
        _isActiveCounting = true;
        _timer = allowedTimeBetweenTaps;
        while (_timer > 0)
        {
            _timer -= Time.deltaTime;
            yield return null;
        }

        _isActiveCounting = false;
        _timer = 0;
        _countedTaps = 0;
    }

    private void UnlockGame()
    {
        transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);
        ProgressValues.checkPointScene = storyDataStorage.GetSceneState(storyDataStorage.GetHighScoreLevelIndex());
    }
}
