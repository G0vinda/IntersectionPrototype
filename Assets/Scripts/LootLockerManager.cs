using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using System;
using Character;
using LLlibs.ZeroDepJson;

public class LootLockerManager : MonoBehaviour
{
    public static LootLockerManager Instance 
    {
        get 
        {
            if(instance == null)
            {
                var go = new GameObject("LootLockerManager");
                instance = go.AddComponent<LootLockerManager>();
            }

            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    public Action playerLoggedIn;
    public Action scoreGotUploaded;
    public Action<List<ScoringSystem.HighScoreEntryData>> highScoreListFetched;

    public bool loggedIn {get; private set;}

    private static LootLockerManager instance;

    private string _leaderboardKey = "TPHighscore";

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Login()
    {
        StartCoroutine(LoginRoutine());
    }

    public void UpdloadScoreToLeaderboard(ScoringSystem.HighScoreEntryData data)
    {
        StartCoroutine(SubmitScoreRoutine(data));
    }

    public void FetchHighScoreList()
    {
        StartCoroutine(FetchHighScoreListRoutine());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            //UpdloadScoreToLeaderboard("Bob", null, 100);
        }
    }

    private IEnumerator LoginRoutine()
    {
        var done = false;
        LootLockerSDKManager.StartGuestSession((response) => 
        {
            if(response.success)
            {
                Debug.Log("Player was logged in!");
                PlayerPrefs.SetString("DeviceId", response.player_id.ToString());
                playerLoggedIn?.Invoke();
                loggedIn = true;
            }
            else
            {
                Debug.Log("Could not log in...");
            }
            done = true;
        });
        yield return new WaitWhile(() => done == false);
    }

    private IEnumerator SubmitScoreRoutine(ScoringSystem.HighScoreEntryData data)
    {
        var done = false;
        var metaData = data.ToString();
        var playerID = PlayerPrefs.GetInt("LastPlayerId", 0) + 1;
        var SessionID = PlayerPrefs.GetString("DeviceId") + playerID.ToString();
        LootLockerSDKManager.SubmitScore(SessionID, data.score, _leaderboardKey, metaData, (response) =>
        {
            if(response.success)
            {
                Debug.Log("Successfully uploaded score");
                scoreGotUploaded?.Invoke();
                PlayerPrefs.SetInt("LastPlayerId", playerID);
            }
            else
            {
                Debug.Log("Failed uploading score: " + response.errorData.ToString());
            }
            done = true;
        });
        yield return new WaitWhile(() => done == false);
    }

    private IEnumerator FetchHighScoreListRoutine()
    {
        var done = false;
        LootLockerSDKManager.GetScoreList(_leaderboardKey, 100, 0, (response) =>
        {
            if(response.success)
            {
                var highScoreData = new List<ScoringSystem.HighScoreEntryData>();
                foreach (var item in response.items)
                {
                    highScoreData.Add(ScoringSystem.HighScoreEntryData.FromString(item.metadata));
                }
                highScoreListFetched?.Invoke(highScoreData);
            }   
            else
            {
                Debug.Log("Failed to fetch high scores: " + response.errorData.ToString());
            }
            done = true;
        });
        yield return new WaitWhile(() => done == false);
    }
}
