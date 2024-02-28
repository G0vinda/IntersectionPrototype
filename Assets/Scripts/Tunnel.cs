using System.Collections.Generic;
using Character;
using Environment;
using UnityEngine;

[RequireComponent(typeof(Obstacle))]
public class Tunnel : MonoBehaviour
{
    [Header("Roof")]
    [SerializeField] private MeshRenderer primaryRoofPart;
    [SerializeField] private MeshRenderer secondaryRoofPart;

    [Header("Materials")]
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material redMaterial;

    private Obstacle _obstacle;
    private Dictionary<CharacterAttributes.CharColor, Material> _materials;

    private void Awake()
    {
        _obstacle = GetComponent<Obstacle>();
        _materials = new Dictionary<CharacterAttributes.CharColor, Material>()
        {
            {CharacterAttributes.CharColor.Blue, blueMaterial},
            {CharacterAttributes.CharColor.Red, redMaterial}
        };

        _obstacle.AddAllowedColor(CharacterAttributes.CharColor.Blue);
        primaryRoofPart.material = _materials[CharacterAttributes.CharColor.Blue];
    }

    public void SetSecondaryColor(CharacterAttributes.CharColor colorValue)
    {
        _obstacle.AddAllowedColor(colorValue);
        secondaryRoofPart.material = _materials[colorValue];
    }
}
