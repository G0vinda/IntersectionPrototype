using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;

public class RollTrigger : MonoBehaviour
{
    [SerializeField] CharacterRollDialog characterRollDialog;

    private Info info;

    void Start()
    {
        Time.timeScale = 1f;
        info = FlowManager.Instance.GetRollTriggerInfo();
        characterRollDialog.StartRoll(info.spawnRestrictions, info.predeterminedAttributes);
    }

    public class Info
    {
        public CharacterAttributes.SpawnRestrictions spawnRestrictions;
        public CharacterAttributes predeterminedAttributes;
    }
}
