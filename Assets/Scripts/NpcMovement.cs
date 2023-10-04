using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    [SerializeField] private float scaleFactor;
    [SerializeField] private float moveDistance;
    [SerializeField] private float scaleTime;
    [SerializeField] private float moveTime;
    
    void Start()
    {
        var biggerScale = transform.localScale * scaleFactor;
        var moveFactor = Random.Range(0, 1) == 0 ? -1f : 1f;
        var moveVector = transform.position + Vector3.right * moveDistance * moveFactor;
        var npcSequence = DOTween.Sequence();
        npcSequence.Append(transform.DOScale(biggerScale, scaleTime).SetEase(Ease.OutSine).SetLoops(-1, LoopType.Yoyo));
        npcSequence.Join(transform.DOMove(moveVector, moveTime).SetLoops(-1, LoopType.Yoyo));
    }
}
