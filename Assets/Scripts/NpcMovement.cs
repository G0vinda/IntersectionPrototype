using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class NpcMovement : MonoBehaviour
{
    [SerializeField] private float scaleFactor;
    [SerializeField] private float scaleTime;
    [SerializeField] private float moveTime;
    [SerializeField] private float pauseTime;
    [SerializeField] private int pushDistance;
    [SerializeField] private float pushAnimationTime;
    [SerializeField] private float pushAnimationStrength;
    [SerializeField] private SpeechBubble speechBubble;
    [SerializeField] private float speechBubbleShowTime;
    [SerializeField] private float pushDelayAfterAnimationStart;
    [SerializeField] private Animator animator;

    public Vector3[] _wayPoints;
    private int _gridMaxX;
    private int _gridMinX;
    private WaitForSeconds _moveWait;
    private CharacterAttributes.Shape _npcShape;
    private Tween _moveTween;
    private bool _isPushing;
    private bool _justPushed;
    private int _wayPointIndex;
    private int _direction;
    private bool _movesHorizontal;

    public void Initialize(Vector3[] wayPoints, CharacterAttributes.Shape shape)
    {
        _wayPoints = wayPoints;
        _movesHorizontal = wayPoints[1].x != transform.position.x;
        var biggerScale = transform.localScale * scaleFactor;
        transform.DOScale(biggerScale, scaleTime).SetEase(Ease.OutSine).SetLoops(-1, LoopType.Yoyo);
        
        _direction = 1;
        _npcShape = shape;
        
        _moveWait = new WaitForSeconds(moveTime + pauseTime);
        
        speechBubble.Initialize();
        
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.2f)); // this delay makes the npc movements seem not as in sync with each other
        do
        {
            if (_justPushed)
            {
                _justPushed = false;
                yield return new WaitForSeconds(pushAnimationTime + speechBubbleShowTime);
            }

            _wayPointIndex += _direction;
            var destination = _wayPoints[_wayPointIndex];
            _moveTween = transform.DOMove(destination + new Vector3(0, 3f, 0), moveTime).SetEase(Ease.OutSine).OnComplete(() => SetAnimationToIdle());

            if (_wayPointIndex % (_wayPoints.Length - 1) == 0)
                _direction *= -1;
            
            yield return _moveWait;
            SetAnimationToWalking();
        } while (true);
    }

    private void SetAnimationToIdle()
    {
        animator.SetBool("WalkingHorizontal", false);
        animator.SetBool("WalkingVertical", false);
    }

    private void SetAnimationToWalking()
    {
        if(_movesHorizontal)
        {
            animator.SetBool("WalkingHorizontal", true);
            animator.SetBool("WalkingVertical", false);
        }
        else
        {
            animator.SetBool("WalkingHorizontal", false);
            animator.SetBool("WalkingVertical", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isPushing || !other.TryGetComponent<CharacterMovement>(out var characterMovement))
            return;

        var characterAppearance = characterMovement.GetComponent<CharacterAppearance>();
        var characterShape = characterAppearance.GetAttributes().shape;

        if (characterShape == 0 && _npcShape == 0)
        {
            if (characterMovement.RequestPushByNpc())
                StartCoroutine(PerformPushAction(characterShape, characterMovement, true));
        }
        else if (characterShape > _npcShape)
        {
            if (characterMovement.RequestPushByNpc())
                StartCoroutine(PerformPushAction(characterShape, characterMovement, false));
        }
    }

    private IEnumerator PerformPushAction(CharacterAttributes.Shape characterShape, CharacterMovement characterMovement,
        bool pushForward)
    {
        _moveTween.Pause();
        _isPushing = true;
        _justPushed = true;
        
        speechBubble.Show(characterShape, pushForward);
        yield return new WaitForSeconds(speechBubbleShowTime);
        speechBubble.Hide();
        
        transform.DOPunchScale(pushAnimationStrength * Vector3.one, pushAnimationTime).OnComplete(() =>
        {
            _moveTween.Play();
        });

        yield return new WaitForSeconds(pushDelayAfterAnimationStart);
        var pushFactor = pushForward ? 1 : -1;
        characterMovement.PushPlayerByNpc(pushFactor * pushDistance);

        yield return new WaitForSeconds(0.2f); // Make sure not to trigger two push actions after each other
        _isPushing = false;
    }
}