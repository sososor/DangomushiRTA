using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;

public class PlayFabLogin : MonoBehaviour
{
    private static PlayFabLogin instance;

    void Awake()
    {
        // PlayFabLoginのインスタンスが既に存在している場合は、重複して生成しない
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // シーンを切り替えてもオブジェクトを破棄しない
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Login();
    }

    void Login()
    {
        // 毎回新しいIDを生成
        string uniqueId = SystemInfo.deviceUniqueIdentifier + "_" + Random.Range(0, 100000).ToString();

        var request = new LoginWithCustomIDRequest
        {
            CustomId = uniqueId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    public void SubmitName(string playerName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = playerName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, result =>
        {
            Debug.Log("名前の送信成功: " + playerName);  // 送信成功時のログ
        }, error =>
        {
            Debug.LogError("名前の送信失敗: " + error.GenerateErrorReport());  // 送信失敗時のログ
        });
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("ログイン成功！");
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("ログイン失敗: " + error.GenerateErrorReport());
    }

    public void SubmitScore(float time)
    {
        // 時間を秒単位から整数に変換し、-1をかける
        int playerScore = Mathf.FloorToInt(time) * -1;

        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Dangomusi",
                    Value = playerScore
                }
            }
        }, result =>
        {
            Debug.Log($"スコア {playerScore} 送信成功");
        }, error =>
        {
            Debug.LogError("スコア送信失敗: " + error.GenerateErrorReport());
        });
    }

    public void GetLeaderboard(System.Action<GetLeaderboardResult> onSuccess, System.Action<PlayFabError> onFailure)
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = "Dangomusi",
            StartPosition = 0,
            MaxResultsCount = 10
        }, onSuccess, onFailure);
    }
}
