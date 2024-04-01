using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using DG.Tweening;
using UnityEngine;

public class Heli : MonoBehaviour
{
    [SerializeField] float maxForwardSpeed;
    [SerializeField] float maxFlyHeight;
    [SerializeField] float flyTime;
    [SerializeField] private float cityWidth;
    [SerializeField] private float maxHorizontalAcceleration;
    [SerializeField] private float maxHorizontalAccelerationChange;
    [SerializeField] private float maxHorizontalSpeed;
    [SerializeField] private float horizontalDrag;
    [Header("WingRotation")]
    [SerializeField] Transform wingRotator;
    [SerializeField] float idleWingRotationSpeed;
    [SerializeField] float maxWingRotationSpeed;

    public float horizontalSpeed {get; set;}

    private Sequence _collectableTweenSequence;
    private Sequence _startUpSequence;
    private Sequence _landingSequence;
    private float _wingSpeed;
    private float _forwardSpeed;
    private Vector2Int _coordinates;
    private CityGridCreator _cityGridCreator;
    private float _landingHeight;
    private bool _isLanding;
    private float _screenWidth;
    private float _horizontalAcceleration;

    void Update()
    {
        var wingRotation = _wingSpeed * 360f * Time.deltaTime;
        wingRotator.Rotate(new Vector3(0, wingRotation, 0));
    }

    public void GoToCollectableMode()
    {
        transform.localScale = Vector3.one * 0.8f;
        _wingSpeed = idleWingRotationSpeed;
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
        _startUpSequence.Join(transform.DOScale(1f, 0.8f).SetEase(Ease.OutBounce));
        _startUpSequence.Append(DOVirtual.Float(idleWingRotationSpeed, maxWingRotationSpeed, 2f, value => { _wingSpeed = value; }).SetEase(Ease.InCirc));
        _startUpSequence.Join(transform.DOMoveY(maxFlyHeight, 2f).SetEase(Ease.InCirc));
        _startUpSequence.Append(DOVirtual.Float(0, maxForwardSpeed, 0.5f, value => {_forwardSpeed = value;}).SetEase(Ease.InSine));
    }

    private IEnumerator Fly(CameraController cameraController, CharacterMovement characterMovement, float distanceToScorePoint)
    {
        cameraController.SetCamTarget(transform, true);
        var lastScoringZpos = transform.position.z + 0.2f; // the 0.2f are to make sure there is no double scoring at the destination
        _screenWidth = Screen.width;

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

            CalculateHorizontalSpeed();

            var movement = new Vector3(horizontalSpeed, 0, _forwardSpeed) * Time.deltaTime;
            transform.Translate(movement);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, 10, 54), transform.position.y, transform.position.z);

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
        _landingSequence.Append(DOVirtual.Float(_wingSpeed, 0.5f, 1.5f, value => { _wingSpeed = value; }).SetEase(Ease.OutCirc));
        _landingSequence.Join(transform.DOMoveY(_landingHeight, 1.5f).SetEase(Ease.OutSine));
        _landingSequence.OnComplete(() => {
            characterMovement.SetCoordinates(landingCoordinates, true);
            characterMovement.gameObject.SetActive(true);
            cameraController.SetCamTarget(characterMovement.transform);

            Destroy(gameObject);
        });
    }

    private void CalculateHorizontalSpeed()
    {
        if(horizontalSpeed == 0)
        {
            _horizontalAcceleration = 0;
        }

        var inputAcceleration = 0f;
        if(Input.GetMouseButton(0))
        {
            var mouseXOnScreenPosition = Input.mousePosition.x / _screenWidth;
            var heliXPosition = transform.position.x / cityWidth;

            var controlDiff = mouseXOnScreenPosition - heliXPosition;
            inputAcceleration = Mathf.Clamp(controlDiff * maxHorizontalAccelerationChange * Time.deltaTime, -maxHorizontalAccelerationChange, maxHorizontalAccelerationChange);
        }

        var drag = horizontalSpeed > 0 
            ? -horizontalDrag : horizontalSpeed < 0 
                ? horizontalDrag : 0;

        _horizontalAcceleration = Mathf.Clamp(_horizontalAcceleration + inputAcceleration + drag * Time.deltaTime, -maxHorizontalAcceleration, maxHorizontalAcceleration);

        if(inputAcceleration != 0)
        {
            horizontalSpeed = Mathf.Clamp(horizontalSpeed + _horizontalAcceleration * Time.deltaTime, -maxHorizontalSpeed, maxHorizontalSpeed);
        }
        else if(horizontalSpeed > 0)
        {
            _horizontalAcceleration = -horizontalDrag;
            horizontalSpeed = Mathf.Max(horizontalSpeed + _horizontalAcceleration * Time.deltaTime, 0);
        }
        else if(horizontalSpeed < 0)
        {
            _horizontalAcceleration = horizontalDrag;
            horizontalSpeed = Mathf.Min(horizontalSpeed + _horizontalAcceleration * Time.deltaTime, 0);
        }
    }
}
