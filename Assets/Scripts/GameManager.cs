using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PlayFabLogin playFabLogin;  // PlayFabLogin のインスタンスを参照する変数
    public PlayFabLogin2 playFabLogin2;
    public PlayFabLogin3 playFabLogin3;  
    public GameObject mainImage;
   
    public GameObject gameOver;
    public GameObject gameClear;

    public GameObject continueButton;
    public GameObject exitButton;
    public GameObject resultButton;
    
    // HP 管理
    public GameObject heartPrefab;
    public GameObject heartContainer;
    public int maxHP = 10;
    private List<GameObject> hearts = new List<GameObject>();

    public GameObject timerBar;
    public GameObject timerText;
    TimeController timeCnt;
    private float elapsedTime = 0f;
    private bool isTimerRunning = false;
    public  float finalTime;

    Image titleImage;

public static GameManager instance;

    public AudioSource audioSourceA;
    [SerializeField] AudioClip ikikaeriSE;
    public AudioSource audioSourceB;
    [SerializeField] AudioClip gameoverSE;

    public string playerName;


    private void Awake()
    {
        instance = this;
        playerName = PlayerPrefs.GetString("PlayerName", "NoName");
    }

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Invoke("InactiveImage", 1.0f);
        
        continueButton.SetActive(false);
        exitButton.SetActive(true);
        resultButton.SetActive(false);
        

        InitializeHearts();

        elapsedTime = 0f;
        isTimerRunning = false;

        timeCnt = GetComponent<TimeController>();
        if(timeCnt != null){
            if(timeCnt.gameTime == 0.0f){
                timerBar.SetActive(false);
            }
        }
        
        audioSourceA = GetComponent<AudioSource>();
        audioSourceB = GetComponent<AudioSource>();

        // 他のコード...
        if (playFabLogin == null)
        {
            playFabLogin = FindObjectOfType<PlayFabLogin>(); // もしくはプレハブから直接取得
        }
        if (playFabLogin2 == null)
        {
            playFabLogin2 = FindObjectOfType<PlayFabLogin2>(); // もしくはプレハブから直接取得
        }
        if (playFabLogin3 == null)
        {
            playFabLogin3 = FindObjectOfType<PlayFabLogin3>(); // もしくはプレハブから直接取得
        }
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerController playerCnt = player != null ? player.GetComponent<PlayerController>() : null;

        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
           
        }
        if (PlayerController.gameState == "gameclear")
        {
            // ゲームクリア
            if (isTimerRunning)
        {
            isTimerRunning = false;
        }
            mainImage.SetActive(false);
            Button bt = continueButton.GetComponent<Button>();
            bt.interactable = false;
            exitButton.SetActive(true);
            resultButton.SetActive(true);
            gameClear.SetActive(true);
            
            
            PlayerController.gameState = "gameend";
            finalTime = elapsedTime;

            // PlayFabLogin インスタンスを使ってスコアを送信
            if (playFabLogin != null)
            {
                playFabLogin.SubmitScore(finalTime);
            }
            else
            {
                Debug.LogError("PlayFabLogin インスタンスが見つかりません。");
            }
            // PlayFabLogin インスタンスを使ってスコアを送信
            if (playFabLogin2 != null)
            {
                playFabLogin2.SubmitScore(finalTime);
            }
            else
            {
                Debug.LogError("PlayFabLogin2 インスタンスが見つかりません。");
            }
            // PlayFabLogin インスタンスを使ってスコアを送信
            if (playFabLogin3 != null)
            {
                playFabLogin3.SubmitScore(finalTime);
            }
            else
            {
                Debug.LogError("PlayFabLogin3 インスタンスが見つかりません。");
            }

            if(timeCnt != null){
                timeCnt.isTimeOver = true;
            }
        }
        else if (PlayerController.gameState == "gameover")
        {   // ゲームオーバー
            audioSourceB.PlayOneShot(gameoverSE);
            
            mainImage.SetActive(false);
            gameClear.SetActive(false);
            gameOver.SetActive(true);
           //continueButton.SetActive(true);
            resultButton.SetActive(false);
           
        // タイマーのチェック
    if (timeCnt != null && timeCnt.displayTime <= 0.999999999f)
    {
        // タイマーが0または0以下の場合、continueButtonを非表示にする
        continueButton.SetActive(false);
    }
    else
    {
        // タイマーが残っている場合、continueButtonを表示する
        continueButton.SetActive(true);
    }
        


             // HPプレハブを破壊
        foreach (GameObject heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();
            
            PlayerController.gameState = "gameend";

            if(timeCnt != null){
                timeCnt.isTimeOver = true;
            }
        }
        else if (PlayerController.gameState == "playing")
        {
             // ゲーム中
            if (!isTimerRunning)
            {
                isTimerRunning = true;
                elapsedTime = 0f;
            }

            if (playerCnt != null)
            {
                if (playerCnt.CurrentHP != hearts.Count)
                {
                    UpdateHearts(playerCnt.CurrentHP);
                }
            }
            
            if(timeCnt != null){
                if(timeCnt.gameTime > 0.0f){
                    int time = (int)timeCnt.displayTime;
                    timerText.GetComponent<Text>().text = time.ToString();
                    if(time == 0){
                        playerCnt.GameOver();
                    }
                }
            }
            continueButton.SetActive(false);
            gameOver.SetActive(false);
            gameClear.SetActive(false);
            resultButton.SetActive(false);
            exitButton.SetActive(true);
            
        }

        
    }

    void InactiveImage()
    {
        mainImage.SetActive(false);
    }

    public void InitializeHearts()
    {
        foreach (GameObject heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();

        for (int i = 0; i < maxHP; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer.transform);
            hearts.Add(heart);
        }
    }

    public void UpdateHearts(int currentHP)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetActive(i < currentHP);
        }
    }

    

    public void RespawnPlayer()
    {   
        audioSourceA.PlayOneShot(ikikaeriSE);
        InitializeHearts();
       
        Debug.Log("RespawnPlayer method called.");
        PlayerController playerCnt = FindObjectOfType<PlayerController>();
        if (playerCnt != null)
        {
            Debug.Log("Player object found, proceeding to respawn.");
            // playerCnt.CurrentHP = maxHP;  // プレイヤーのHPを最大値にリセット
        UpdateHearts(playerCnt.CurrentHP);  // ハートの表示を更新
            playerCnt.Respawn();  // リスポーン処理を呼び出す
             // タイマーをリセットせずに再スタート
        if (timeCnt.isTimeOver)
        {
            timeCnt.isTimeOver = false;  // タイマーを再び動かす
        }
        isTimerRunning = true;  // タイマーの再開
        }
        else
        {
            Debug.LogError("Player object not found.");
        }
    }

   // public void GameOver()
   // {
    //   mainImage.SetActive(true);
    //   gameOver.SetActive(true);
     //  respawnUI.SetActive(true);
   // }
}
