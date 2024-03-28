using System.Collections;
using Character;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineAnimate))]
public class Ubahn : MonoBehaviour
{
    private SplineAnimate _splineAnimate;

    void Awake()
    {
        _splineAnimate = GetComponent<SplineAnimate>();
    }

    public void Initialize(
        UbahnData data)
    {
        _splineAnimate.Container = data.splineContainer;
        StartCoroutine(ScoreByDistance(data));
    }

    private IEnumerator ScoreByDistance(UbahnData data)
    {
        float zDistance;
        var lastScoringDistance = data.uBahnDestination.z - transform.position.z - 0.2f; // the 0.2f are to make sure there is no double scoring at the destination
        
        yield return null; // without this 'pause' frame the animation doesn't start
        _splineAnimate.Play();

        while (true)
        {
            zDistance = data.uBahnDestination.z - transform.position.z;
            if(zDistance < lastScoringDistance - data.distanceToScorePoint)
            {
                data.characterMovement.IncrementScore();
                lastScoringDistance = zDistance;
            }

             if(!_splineAnimate.IsPlaying)
                break;

            yield return null;
        }

        data.characterMovement.SetCoordinates(data.playerDestination, true);
        data.cameraController.SetCamTarget(data.characterMovement.transform);
        data.characterMovement.gameObject.SetActive(true);
        data.characterMovement.IncrementScore();
        Destroy(gameObject);
    }

    public struct UbahnData
    {
        public SplineContainer splineContainer;
        public Vector3 uBahnDestination; 
        public Vector2Int playerDestination; 
        public CameraController cameraController; 
        public CharacterMovement characterMovement; 
        public float distanceToScorePoint;

        public UbahnData(
            SplineContainer splineContainer,
            Vector3 uBahnDestination,
            Vector2Int playerDestination,
            CameraController cameraController,
            CharacterMovement characterMovement,
            float distanceToScorePoint)
        {
            this.splineContainer = splineContainer;
            this.uBahnDestination = uBahnDestination;
            this.playerDestination = playerDestination;
            this.cameraController = cameraController;
            this.characterMovement = characterMovement;
            this.distanceToScorePoint = distanceToScorePoint;   
        }
    }
}
