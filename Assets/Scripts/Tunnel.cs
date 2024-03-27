using System.Collections.Generic;
using Character;
using Environment;
using UnityEngine;

[RequireComponent(typeof(Obstacle))]
public class Tunnel : MonoBehaviour, IBuildingGroupable
{
    [SerializeField] private MeshRenderer houseRenderer;

    [Header("Roof")]
    [SerializeField] private MeshRenderer primaryRoofPart;
    [SerializeField] private MeshRenderer secondaryRoofPart;

    [Header("Materials")]
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material redMaterial;

    private Obstacle _obstacle;
    private Dictionary<CharacterAttributes.Color, Material> _materials;

    private void Awake()
    {
        _obstacle = GetComponent<Obstacle>();
        _materials = new Dictionary<CharacterAttributes.Color, Material>()
        {
            {CharacterAttributes.Color.Blue, blueMaterial},
            {CharacterAttributes.Color.Red, redMaterial}
        };

        _obstacle.AddAllowedColor(CharacterAttributes.Color.Blue);
        primaryRoofPart.material = _materials[CharacterAttributes.Color.Blue];
    }

    public void SetSecondaryColor(CharacterAttributes.Color colorValue)
    {
        _obstacle.AddAllowedColor(colorValue);
        secondaryRoofPart.material = _materials[colorValue];
    }

    public void SetMaterial(Material newMaterial)
    {
        houseRenderer.material = newMaterial;
    }
}
