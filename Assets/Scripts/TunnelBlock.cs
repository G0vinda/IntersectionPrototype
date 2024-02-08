using Character;
using Environment;
using UnityEngine;

[RequireComponent(typeof(Obstacle))]
public class TunnelBlock : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] primaryColorStripes;
    [SerializeField] private MeshRenderer[] secondaryColorStripes;

    private Obstacle _obstacle;

    private void Awake()
    {
        _obstacle = GetComponent<Obstacle>();
    }

    public void SetPrimaryStripeColor(Color primaryColor, CharacterAttributes.CharColor colorValue)
    {
        _obstacle.AddAllowedColor(colorValue);
        foreach (var stripe in primaryColorStripes)
        {
            stripe.material.color = primaryColor;
        }
    }

    public void SetSecondaryStripeColor(Color secondaryColor, CharacterAttributes.CharColor colorValue)
    {
        _obstacle.AddAllowedColor(colorValue);
        foreach (var stripe in secondaryColorStripes)
        {
            stripe.material.color = secondaryColor;
        }
    }
}
