using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load()
    {
        SceneManager.LoadScene(sceneName);
    }

    // リスポーンプレイヤー関数を呼び出す
    public void RespawnPlayerBBB()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RespawnPlayer();
        }
        else
        {
            Debug.LogError("GameManager instance not found.");
        }
    }
}
