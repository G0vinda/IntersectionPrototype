using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.UI;

public class CharacterRollDialog : MonoBehaviour
{
    [SerializeField] CharacterAppearance rollAppearance;
    [SerializeField] private float rollPause;
    [SerializeField] private float rollTime;
    [SerializeField] private Button startLevelButton;
    
    private CityLevel _cityLevel;
    private CharacterAttributes _lastRollsAttributes = null;

    public void StartRoll(CityLevel cityLevel, CharacterAttributes.SpawnRestrictions spawnRestrictions)
    {
        Debug.Log("Roll should start");
        _cityLevel = cityLevel;
        StartCoroutine(Roll(spawnRestrictions));
    }

    private IEnumerator Roll(CharacterAttributes.SpawnRestrictions spawnRestrictions)
    {
        var timer = rollTime;

        startLevelButton.interactable = false;
        while (timer > 0)
        {
            var randomAttributes = CharacterAttributes.GetRandomAttributes(spawnRestrictions, _lastRollsAttributes);
            rollAppearance.SetAttributes(randomAttributes);
            _lastRollsAttributes = randomAttributes;

            yield return new WaitForSeconds(rollPause);
            timer -= rollPause;
        }

        startLevelButton.interactable = true;
    }

    public void StartLevelClicked()
    {
        _cityLevel.StartLevel(_lastRollsAttributes);
    }
}
