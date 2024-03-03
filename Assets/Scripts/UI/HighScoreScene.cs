using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoreScene : MonoBehaviour
{
    [SerializeField] UIHighScoreEntryList highScoreList;

    void Start()
    {
        highScoreList.Initialize(null);
    }

    public void GoBackToTitleMenu()
    {
        SceneManager.LoadScene("TitleMenu");
    }
}
