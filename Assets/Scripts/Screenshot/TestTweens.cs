using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class TestTweens : MonoBehaviour
{

    [SerializeField] Transform newScale;
    // Start is called before the first frame update
    void OnEnable()
    {

        //Runterbeugen bei Tunnel

        //Tween _ScalingTween = transform.DOScaleY(0.6f, .2f).SetEase(Ease.InOutBack);
        //_ScalingTween.Play();
        //_ScalingTween.OnComplete(() =>
        //{
        //    transform.DOScaleY(2.21642f, .2f).SetEase(Ease.InOutBack);
        //});


        // Window Closing Anim

        transform.DOScale(Vector3.zero, .4f).SetEase(Ease.InQuad);
        transform.DOLocalMoveY(-1200, .4f).SetEase(Ease.InQuad);
        transform.DOLocalMoveX(-400, .4f).SetEase(Ease.InQuad);

        // Button Anim

        //transform.DOScale(Vector3.zero, .25f).SetEase(Ease.InOutBack);


        // Morph
        // basically change the height and position data to another, and then the other way around
        //transform.Do(newScale.localScale, 1f);
        //DOTween.To()

    }


}
