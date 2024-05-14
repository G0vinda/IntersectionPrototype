using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;

public class HeliArea : MonoBehaviour
{
    [SerializeField] Heli heliPrefab;
    [SerializeField] float heliFloorOffset;

    private CameraController _cameraController;
    private float _distanceToScorePoint;
    private Heli _heli;
    private Vector2Int _coordinates;
    private CityGridCreator _cityGridCreator;

    public void Initialize(CameraController cameraController, float distanceToScorePoint, Vector2Int coordinates, CityGridCreator cityGridCreator)
    {
        _cameraController = cameraController;
        _distanceToScorePoint = distanceToScorePoint;
        _coordinates = coordinates;
        _cityGridCreator = cityGridCreator;
        _heli = Instantiate(heliPrefab, transform.position + Vector3.up * heliFloorOffset, Quaternion.identity);
        _heli.GoToCollectableMode();
    }

    void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent<CharacterMovement>(out var characterMovement) || _heli == null)
            return;
        
        var attributes = characterMovement.GetComponent<CharacterAppearance>().GetAttributes();
        if(attributes.pattern != CharacterAttributes.Pattern.Check)
        {
            Debug.Log("You can only fly Heli with a check pattern!");
            return;
        }

        characterMovement.gameObject.SetActive(false);
        _heli.GoToFlyMode(_cameraController, characterMovement, _distanceToScorePoint, _coordinates, _cityGridCreator);
    }
}
