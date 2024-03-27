using System.Collections;
using System.Collections.Generic;
using Character;
using DG.Tweening;
using UnityEngine;

public class Ubahn : MonoBehaviour
{
    [SerializeField] private float travelDuration;

    public void StartDriving(Vector3 uBahnDestination, Vector2Int playerDestination, CameraController cameraController, CharacterMovement characterMovement, float distanceToScorePoint)
    {
        StartCoroutine(ScoreByDistance(distanceToScorePoint, uBahnDestination, characterMovement));
        transform.DOMove(uBahnDestination, travelDuration).SetEase(Ease.InOutSine).OnComplete(() => {
            characterMovement.SetCoordinates(playerDestination);
            cameraController.SetCamTarget(characterMovement.transform);
            characterMovement.gameObject.SetActive(true);
            characterMovement.IncrementScore();
            Destroy(gameObject);
        });
    }

    private IEnumerator ScoreByDistance(float distanceToScore, Vector3 destination, CharacterMovement characterMovement)
    {
        float zDistance;
        var lastScoringDistance = destination.z - transform.position.z - 0.2f; // the 0.2f are to make sure there is no double scoring at the destination
        while (true)
        {
            zDistance = destination.z - transform.position.z;
            if(zDistance < lastScoringDistance - distanceToScore)
            {
                characterMovement.IncrementScore();
                lastScoringDistance = zDistance;
            }

            yield return null;
        }
    }
}
