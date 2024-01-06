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

    private Dictionary<CharacterAttributes.CharShape, Sprite> _shapeIconSprites;
    private UnityEngine.Camera _camera;

    public void Initialize()
    {
        _shapeIconSprites = new Dictionary<CharacterAttributes.CharShape, Sprite>()
        {
            { CharacterAttributes.CharShape.Cube, squareIcon },
            { CharacterAttributes.CharShape.Pyramid, triangleIcon },
            { CharacterAttributes.CharShape.Sphere, circleIcon },
        };
    }

    public void Show(CharacterAttributes.CharShape shape, bool positive = false)
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
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        overlayIconImage.gameObject.SetActive(false);
    }
}