using Character;
using UnityEngine;

public class CharacterShape : MonoBehaviour
{
    [SerializeField] GameObject linePattern;
    [SerializeField] GameObject checkPattern;

    public void SetPattern(CharacterAttributes.Pattern pattern)
    {
        switch (pattern)
        {
            case CharacterAttributes.Pattern.Check:
                checkPattern.SetActive(true);
                linePattern.SetActive(false);
                break;
            case CharacterAttributes.Pattern.Lined:
                checkPattern.SetActive(false);
                linePattern.SetActive(true);
                break;
            case CharacterAttributes.Pattern.None:
                checkPattern.SetActive(false);
                linePattern.SetActive(false);
                break;
        }
    }
}
