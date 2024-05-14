using DG.Tweening;
using UnityEngine;

public abstract class CharacterMovement : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected float moveTime;
    [SerializeField] protected ParticleSystem streetParticles;

    protected Tween _moveTween;

    protected void SetAnimationToIdle()
    {
        animator.SetBool("WalkingHorizontal", false);
        animator.SetBool("WalkingVertical", false);
    }

    protected void SetAnimationToWalking(bool horizontal)
    {
        animator.SetBool("WalkingHorizontal", horizontal);
        animator.SetBool("WalkingVertical", !horizontal);
    }

    protected void Move(Vector3 destination, bool moveHorizontal, TweenCallback afterMove)
    {
        var startPosition = transform.position;

        _moveTween = DOVirtual.Float(0, 1f, moveTime, value => {
            transform.position = Vector3.Lerp(startPosition, destination, value);
            animator.speed = Mathf.Lerp(1, 0.75f, value);
        }).SetEase(Ease.OutSine).OnComplete(afterMove);
        
        streetParticles.Play();
        SetAnimationToWalking(moveHorizontal);
    }
}
