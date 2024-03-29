using System;
using System.Collections.Generic;
using Character;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    [Header("IconSprites")] 
    [SerializeField] private Sprite squareIcon;
    [SerializeField] private Sprite triangleIcon;
    [SerializeField] private Sprite circleIcon;
    [SerializeField] private Sprite redCrossIcon;
    [SerializeField] private Sprite greenCheckIcon;

    [Header("ImageReferences")] 
    [SerializeField] private Image shapeIconImage;
    [SerializeField] private Image overlayIconImage;

    [Header("AnimationValues")] 
    [SerializeField] private float punchScaleFactor;
    [SerializeField] private float punchScaleTime;
    [SerializeField] private float overlayIconDelayTime;
    [SerializeField] private float overlayIconAnimationTime;

    private Dictionary<CharacterAttributes.Shape, Sprite> _shapeIconSprites;
    private UnityEngine.Camera _camera;

    public void Initialize()
    {
        _shapeIconSprites = new Dictionary<CharacterAttributes.Shape, Sprite>()
        {
            { CharacterAttributes.Shape.Cube, squareIcon },
            { CharacterAttributes.Shape.Pyramid, triangleIcon },
            { CharacterAttributes.Shape.Sphere, circleIcon },
        };
    }

    public void Show(CharacterAttributes.Shape shape, bool positive = false)
    {
        transform.parent.rotation = Quaternion.LookRotation(transform.parent.position - UnityEngine.Camera.main.transform.position);
        shapeIconImage.sprite = _shapeIconSprites[shape];
        transform.DOPunchScale(Vector3.one * punchScaleFactor, punchScaleTime);
        gameObject.SetActive(true);
        
        overlayIconImage.sprite = positive ? greenCheckIcon : redCrossIcon;
        Invoke(nameof(ShowOverlayIcon), overlayIconDelayTime);
    }

    private void ShowOverlayIcon()
    {
        overlayIconImage.gameObject.SetActive(true);
        overlayIconImage.transform.DOPunchScale(Vector3.one * 0.2f, overlayIconAnimationTime);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        overlayIconImage.gameObject.SetActive(false);
    }
}