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
    
    private CharacterAttributes _lastRollsAttributes = null;

    public void StartRoll(CharacterAttributes.SpawnRestrictions spawnRestrictions, CharacterAttributes predeterminedAttributes)
    {
        StartCoroutine(Roll(spawnRestrictions, predeterminedAttributes));
    }

    private IEnumerator Roll(CharacterAttributes.SpawnRestrictions spawnRestrictions, CharacterAttributes predeterminedAttributes)
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

        var attributesFromLastLevel = FlowManager.Instance.currentCharacterAttributes;
        if(attributesFromLastLevel != null)
        {
            _lastRollsAttributes = CharacterAttributes.GetRandomAttributes(spawnRestrictions, attributesFromLastLevel);
            rollAppearance.SetAttributes(_lastRollsAttributes);
        }

        if(predeterminedAttributes != null)
        {
            _lastRollsAttributes = predeterminedAttributes;
            rollAppearance.SetAttributes(predeterminedAttributes);
        }

        startLevelButton.interactable = true;
    }

    public void StartLevelClicked()
    {
        FlowManager.Instance.currentCharacterAttributes = _lastRollsAttributes;
        FlowManager.Instance.ContinueClicked();
    }
}
