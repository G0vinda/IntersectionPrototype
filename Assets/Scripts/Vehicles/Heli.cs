using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using DG.Tweening;
using UnityEngine;

public class Heli : MonoBehaviour
{
    [SerializeField] Transform wingRotator;
    [SerializeField] float maxForwardSpeed;
    [SerializeField] float maxHorizontalSpeed;
    [SerializeField] float maxFlyHeight;
    [SerializeField] float flyTime;

    private Sequence _collectableTweenSequence;
    private Sequence _startUpSequence;
    private Sequence _landingSequence;
    private float _wingSpeed;
    private float _forwardSpeed;
    private Vector2Int _coordinates;
    private CityGridCreator _cityGridCreator;
    private float _landingHeight;
    private bool _isLanding;

    public void GoToCollectableMode()
    {
        transform.localScale = Vector3.one * 0.6f;

        _collectableTweenSequence = DOTween.Sequence();
        _collectableTweenSequence.Append(transform.DORotate(new Vector3(0, 360, 0), 3, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        _collectableTweenSequence.Join(transform.DOMoveY(2f, 1.5f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo));
        _collectableTweenSequence.SetLoops(-1, LoopType.Restart);
    }

    public void GoToFlyMode(CameraController cameraController, CharacterMovement characterMovement, float distanceToScorePoint, Vector2Int coordinates, CityGridCreator cityGridCreator)
    {
        _coordinates = coordinates;
        _cityGridCreator = cityGridCreator;
        _landingHeight = transform.position.y;
        _collectableTweenSequence?.Kill();
        StartCoroutine(Fly(cameraController, characterMovement, distanceToScorePoint));

        _startUpSequence = DOTween.Sequence();
        _startUpSequence.Append(transform.DORotateQuaternion(Quaternion.identity, 0.5f));
        _startUpSequence.Join(transform.DOScale(1f, 1f).SetEase(Ease.OutBounce));
        _startUpSequence.Append(DOVirtual.Float(0, 4, 3f, value => { _wingSpeed = value; }).SetEase(Ease.InCirc));
        _startUpSequence.Join(transform.DOMoveY(maxFlyHeight, 3f).SetRelative(true).SetEase(Ease.InCirc));
        _startUpSequence.Append(DOVirtual.Float(0, maxForwardSpeed, 0.5f, value => {_forwardSpeed = value;}).SetEase(Ease.InSine));
    }

    private IEnumerator Fly(CameraController cameraController, CharacterMovement characterMovement, float distanceToScorePoint)
    {
        cameraController.SetCamTarget(transform);
        var lastScoringZpos = transform.position.z + 0.2f; // the 0.2f are to make sure there is no double scoring at the destination
        float wingRotation;

        while (true)
        {
            var zPos = transform.position.z;
            if(zPos - lastScoringZpos > distanceToScorePoint)
            {
                characterMovement.IncrementScore();
                lastScoringZpos = transform.position.z;
                
                _coordinates += Vector2Int.up;
                _cityGridCreator.TryGetIntersectionPosition(_coordinates, out _); // call this to generate needed rows
            }

            wingRotation = _wingSpeed * 360f * Time.deltaTime;
            wingRotator.Rotate(new Vector3(0, wingRotation, 0));

            var forwardMovement = _forwardSpeed * Time.deltaTime;
            transform.Translate(new Vector3(0, 0, forwardMovement));

            flyTime -= Time.deltaTime;

            if(flyTime <= 0 && !_isLanding)
                InitiateLanding(cameraController, characterMovement);

            yield return null;
        }
    }

    private void InitiateLanding(CameraController cameraController, CharacterMovement characterMovement)
    {
        _isLanding = true;
        _forwardSpeed = 0;

        var landingCoordinates = _cityGridCreator.GetIntersectionPositionForHeliLanding(_coordinates + Vector2Int.up, transform.position);
        _cityGridCreator.TryGetIntersectionPosition(landingCoordinates, out var landingPosition);
        landingPosition.y = transform.position.y;

        _landingSequence = DOTween.Sequence();
        _landingSequence.Append(transform.DOMove(landingPosition, 0.5f).SetEase(Ease.OutSine));
        _landingSequence.Append(DOVirtual.Float(_wingSpeed, 0.5f, 3f, value => { _wingSpeed = value; }).SetEase(Ease.OutCirc));
        _landingSequence.Join(transform.DOMoveY(_landingHeight, 3f).SetEase(Ease.OutSine));
        _landingSequence.OnComplete(() => {
            characterMovement.SetCoordinates(landingCoordinates, true);
            characterMovement.gameObject.SetActive(true);
            cameraController.SetCamTarget(characterMovement.transform);

            Destroy(gameObject);
        });
    }
}
