using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;

public class UbahnStation : MonoBehaviour
{
    [SerializeField] private Ubahn UBahnPrefab;
    [SerializeField] private Transform UBahnPoint;
    [SerializeField] private List<MeshRenderer> renderers;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material transparentMaterial;

    private bool _isEntry;
    private UbahnStation _endStation;
    private Vector2Int _endCoordinates;
    private CameraController _cameraController;
    private float _distanceToScorePoint;

    public void InitializeAsEntry(UbahnStation endStation, Vector2Int endCoordinates, CameraController cameraController, float distanceToScorePoint)
    {
        _isEntry = true;
        _endStation = endStation;
        _endCoordinates = endCoordinates;
        _cameraController = cameraController;
        _distanceToScorePoint = distanceToScorePoint;
    }

    public Vector3 GetUBahnPointPosition()
    {
        return UBahnPoint.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent<CharacterMovement>(out var characterMovement))
            return;

        renderers.ForEach(renderer => renderer.material = transparentMaterial);

        if(!_isEntry)
            return;
        
        var attributes = characterMovement.GetComponent<CharacterAppearance>().GetAttributes();
        if(attributes.pattern == CharacterAttributes.Pattern.None)
        {
            Debug.Log("You can only drive UBahn with a pattern!");
            return;
        }

        var uBahn = Instantiate(UBahnPrefab, UBahnPoint.position, Quaternion.identity);
        _cameraController.SetCamTarget(uBahn.transform);
        other.gameObject.SetActive(false);

        uBahn.StartDriving(_endStation.GetUBahnPointPosition(), _endCoordinates, _cameraController, characterMovement, _distanceToScorePoint);
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.TryGetComponent<CharacterMovement>(out var characterMovement))
            return;

        renderers.ForEach(renderer => renderer.material = normalMaterial);
    }
}
