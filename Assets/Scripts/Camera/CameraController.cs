using System;
using System.Collections;
using Cinemachine;
using Environment;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float shakeTime;
    [SerializeField] private float shakeIntensity;
    
    private CinemachineVirtualCamera _camera;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private bool _cameraIsShaking;

    private void Awake()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();
        _perlin = _camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _perlin.m_AmplitudeGain = 0f;
    }

    #region OnEnable/OnDisable

    private void OnEnable()
    {
        Obstacle.CharacterCollided += StartCameraShake;
    }
    
    private void OnDisable()
    {
        Obstacle.CharacterCollided -= StartCameraShake;
    }

    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCameraShake();
        }
    }

    private void StartCameraShake()
    {
        StartCoroutine(ShakeCamera());
    }

    private IEnumerator ShakeCamera()
    {
        if (_cameraIsShaking)
            yield break;

        _cameraIsShaking = true;
        _perlin.m_AmplitudeGain = shakeIntensity;
                
        yield return new WaitForSeconds(shakeTime);

        _cameraIsShaking = false;
        _perlin.m_AmplitudeGain = 0f;
    }
}
