using Character;
using UnityEngine;

public class CharacterShape : MonoBehaviour
{
    [SerializeField] GameObject linePattern;
    [SerializeField] GameObject checkPattern;

    public void SetPattern(CharacterAttributes.CharPattern pattern)
    {
        switch (pattern)
        {
            case CharacterAttributes.CharPattern.Check:
                checkPattern.SetActive(true);
                linePattern.SetActive(false);
                break;
            case CharacterAttributes.CharPattern.Lined:
                checkPattern.SetActive(false);
                linePattern.SetActive(true);
                break;
            case CharacterAttributes.CharPattern.None:
                checkPattern.SetActive(false);
                linePattern.SetActive(false);
                break;
        }
    }
}
