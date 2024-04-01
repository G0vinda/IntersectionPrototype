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
                if(checkPattern != null)
                    checkPattern.SetActive(true);
                
                if(linePattern != null)
                    linePattern.SetActive(false);
                break;
            case CharacterAttributes.Pattern.Lined:
                if(checkPattern != null)
                    checkPattern.SetActive(false);

                if(linePattern != null)
                    linePattern.SetActive(true);
                break;
            case CharacterAttributes.Pattern.None:
                if(checkPattern != null)
                    checkPattern.SetActive(false);

                if(linePattern != null)
                    linePattern.SetActive(false);
                break;
        }
    }
}
