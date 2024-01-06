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

    private Vector2Int _direction;
    private CityGridCreator _cityGrid;
    private Vector2Int _currentCoordinates;
    private int _gridMaxX;
    private int _gridMinX;
    private WaitForSeconds _moveWait;
    private CharacterAttributes.CharShape _npcShape;
    private Tween _moveTween;
    private bool _isPushing;
    private bool _justPushed;

    public void Initialize(Vector2Int coordinates, CityGridCreator cityGridCreator, CharacterAttributes.CharShape shape)
    {
        var biggerScale = transform.localScale * scaleFactor;
        transform.DOScale(biggerScale, scaleTime).SetEase(Ease.OutSine).SetLoops(-1, LoopType.Yoyo);

        _direction = Random.Range(0, 2) == 0 ? Vector2Int.left : Vector2Int.right;
        _currentCoordinates = coordinates;
        _npcShape = shape;

        _cityGrid = cityGridCreator;
        (_gridMinX, _gridMaxX) = _cityGrid.GetCurrentXBounds();
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
            
            var newCoordinates = _currentCoordinates + _direction;
            if (newCoordinates.x > _gridMaxX || newCoordinates.x < _gridMinX)
            {
                _direction = -_direction;
                newCoordinates = _currentCoordinates + _direction;
            }

            _cityGrid.TryGetIntersectionPosition(newCoordinates, out var destination);
            _moveTween = transform.DOMove(destination + new Vector3(0, 3f, 0), moveTime).SetEase(Ease.OutSine);
            _currentCoordinates = newCoordinates;
            
            yield return _moveWait;
        } while (true);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger on NPC entered!");
        if (_isPushing || !other.TryGetComponent<CharacterMovement>(out var characterMovement))
            return;

        var characterAttributes = characterMovement.GetComponent<CharacterAttributes>();
        var characterShape = characterAttributes.GetShape();

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

    private IEnumerator PerformPushAction(CharacterAttributes.CharShape characterShape, CharacterMovement characterMovement,
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