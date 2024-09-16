using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class ResultSceneManager : MonoBehaviour
{
    public Text timeText;                // タイム表示用のテキスト
    public ScrollRect scrollRect;        // スクロールビュー
    public GameObject leaderboardItemPrefab; // リーダーボードアイテムのプレハブ

    private PlayFabLogin playFabLogin;
    private RectTransform contentRectTransform;

    void Start()
{
    playFabLogin = FindObjectOfType<PlayFabLogin>();

    if (playFabLogin == null)
    {
        Debug.LogError("PlayFabLogin インスタンスが見つかりません。");
        return;
    }

    float time = GameManager.instance.finalTime;

    // timeをそのまま秒として使う
    int seconds = Mathf.FloorToInt(time);
    int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

    // 秒数とミリ秒の形式に変更
    timeText.text = string.Format("Time: {0:00}:{1:000}", seconds, milliseconds);

    // スクロールビューのContentを取得
    contentRectTransform = scrollRect.content;

    // 正しいGetLeaderboardメソッドを呼び出す
    playFabLogin.GetLeaderboard(OnLeaderboardSuccess, OnLeaderboardFailure);
}


    void OnLeaderboardSuccess(GetLeaderboardResult result)
    {
        // 既存のリーダーボードアイテムをクリア
        foreach (Transform child in contentRectTransform)
        {
            Destroy(child.gameObject);
        }

        // プレイヤー名を取得
        string savedPlayerName = PlayerPrefs.GetString("PlayerName", "NoName");

        // スコアを小さい順にソートするために、リーダーボードアイテムをソート
        var sortedLeaderboard = new List<PlayerLeaderboardEntry>(result.Leaderboard);
        sortedLeaderboard.Sort((a, b) => a.StatValue.CompareTo(b.StatValue));

        foreach (var item in sortedLeaderboard)
        {
            // リーダーボードアイテムのプレハブをインスタンス化
            GameObject leaderboardItem = Instantiate(leaderboardItemPrefab, contentRectTransform);

            // アイテムのテキストコンポーネントを取得
            Text leaderboardText = leaderboardItem.GetComponentInChildren<Text>();

            if (leaderboardText != null)
            {
                // スコアに -1 をかけて表示
                int adjustedScore = item.StatValue * -1;

                // テキストを設定
                string displayName = item.DisplayName ?? "NoName";
                
                // 保存されたプレイヤー名と一致する場合にプレイヤー名を反映
                if (displayName == savedPlayerName)
                {
                    displayName = savedPlayerName;
                }

                leaderboardText.text = $"{item.Position + 1}位: {displayName}: {adjustedScore}秒";
            }
            else
            {
                Debug.LogError("プレハブに Text コンポーネントが見つかりません。");
            }
        }
    }

    void OnLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError("リーダーボード取得失敗: " + error.GenerateErrorReport());
        timeText.text = "リーダーボード取得失敗";
    }
}
