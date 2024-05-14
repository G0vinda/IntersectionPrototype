using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.Splines;

public class UbahnStation : MonoBehaviour
{
    [SerializeField] private Ubahn uBahnPrefab;
    [SerializeField] private UBahnRails uBahnRailsPrefab;
    [SerializeField] private List<MeshRenderer> renderers;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private float railYOffset;

    private bool _isEntry;
    private UbahnStation _endStation;
    private Vector2Int _endCoordinates;
    private CameraController _cameraController;
    private float _distanceToScorePoint;
    private SplineContainer _railSpline;

    public void InitializeAsEntry(
        UbahnStation endStation, 
        Vector2Int endCoordinates, 
        CameraController cameraController, 
        float distanceToScorePoint,
        Vector3[] relativeRailKnotPositions)
    {
        _isEntry = true;
        _endStation = endStation;
        _endCoordinates = endCoordinates;
        _cameraController = cameraController;
        _distanceToScorePoint = distanceToScorePoint;

        var rails = Instantiate(uBahnRailsPrefab, transform.position + new Vector3(0, railYOffset, 0), Quaternion.identity);
        _railSpline = rails.BuildRails(relativeRailKnotPositions);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent<PlayerMovement>(out var characterMovement))
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

        var uBahn = Instantiate(uBahnPrefab, _railSpline.transform);
        _cameraController.SetCamTarget(uBahn.transform);
        other.gameObject.SetActive(false);

        uBahn.Initialize(
            new Ubahn.UbahnData(
                _railSpline,
                _endStation.transform.position, 
                _endCoordinates, 
                _cameraController, 
                characterMovement, 
                _distanceToScorePoint));
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.TryGetComponent<PlayerMovement>(out var characterMovement))
            return;

        renderers.ForEach(renderer => renderer.material = normalMaterial);
    }
}
