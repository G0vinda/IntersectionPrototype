using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;

public class TunnelBlock : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] primaryColorStripes;
    [SerializeField] private MeshRenderer[] secondaryColorStripes;

    public static event Action CharacterCollided;
    
    private List<CharacterAttributes.CharColor> _allowedColors = new ();

    public void SetPrimaryStripeColor(Color primaryColor, CharacterAttributes.CharColor colorValue)
    {
        _allowedColors.Add(colorValue);
        foreach (var stripe in primaryColorStripes)
        {
            stripe.material.color = primaryColor;
        }
    }

    public void SetSecondaryStripeColor(Color secondaryColor, CharacterAttributes.CharColor colorValue)
    {
        _allowedColors.Add(colorValue);
        foreach (var stripe in secondaryColorStripes)
        {
            stripe.material.color = secondaryColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered");
        if (!other.TryGetComponent<CharacterAttributes>(out var characterAttributes))
        {
            return;
        }

        if (!_allowedColors.Contains(characterAttributes.GetColor()))
        {
            CharacterCollided?.Invoke();
            var characterMovement = characterAttributes.GetComponent<CharacterMovement>();
            characterMovement.PushPlayerBackTunnel();
            Debug.Log("Not allowed Color detected!!");    
        }
        else
        {
            Debug.Log("Allowed color detected...");
        }
        
    }
}
