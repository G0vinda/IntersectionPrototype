using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class TestTweens : MonoBehaviour
{

    
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
        // end
        //transform.DOScale(Vector3.zero, .4f).SetEase(Ease.InQuad);
        // start (muss immer am anfang zuerst auf 000 gesetzt werden)
        //transform.localScale = Vector3.zero;
        //transform.DOScale(Vector3.one, .4f).SetEase(Ease.InOutBack);


        // Button Anim

        //transform.DOScale(Vector3.zero, .25f).SetEase(Ease.InOutBack);

        // Slider für das Ingame Menu
        //transform.DOLocalMoveY(transform.localPosition.y + 500, 0.75f).SetEase(Ease.InBack);
        //Position so setzen dass es nicht im Bild ist
        transform.transform.position = new Vector3(transform.position.x, transform.position.y+500, transform.position.z);
        transform.DOLocalMoveY(transform.localPosition.y - 500, 0.6f).SetEase(Ease.OutExpo);
     

    }


}
