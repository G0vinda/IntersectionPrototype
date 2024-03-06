using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTextSceneData", menuName = "SceneData/TextSceneData", order = 100)]
public class TextSceneData : SceneData
{
    [TextArea]
    public string text;
    public TextSceneData page2;
}
