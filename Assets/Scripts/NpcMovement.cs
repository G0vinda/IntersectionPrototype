using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using DG.Tweening;
using UnityEngine;
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

    private Vector2Int _direction;
    private CityGridCreator _cityGrid;
    private Vector2Int _currentCoordinates;
    private int _gridMaxX;
    private int _gridMinX;
    private WaitForSeconds _moveWait;
    private CharacterAttributes.CharShape _npcShape;
    private Tween _moveTween;
    private bool _didPush;

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

        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.2f)); // this delay makes the npc movement seem not as in sync
        do
        {
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
            if (_didPush)
            {
                _didPush = false;
                yield return new WaitForSeconds(pushAnimationTime);
            }
        } while (true);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger on NPC entered!");
        if(!other.TryGetComponent<CharacterMovement>(out var characterMovement))
            return;

        var characterAttributes = characterMovement.GetComponent<CharacterAttributes>();
        var characterShape = characterAttributes.GetShape();

        var pushed = false;
        if (characterShape > _npcShape)
        {
            pushed = characterMovement.PushPlayerByNpc(-pushDistance);
        }else if (characterShape == 0 && _npcShape == 0)
        {
            pushed = characterMovement.PushPlayerByNpc(pushDistance);
        }

        if (pushed)
        {
            _moveTween.Pause();
            _didPush = true;
            transform.DOPunchScale(pushAnimationStrength * Vector3.one, pushAnimationTime).OnComplete(() =>
            {
                _moveTween.Play();
            });
        }
    }
}
