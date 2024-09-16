using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float leftLimit = 0.0f;
    public float rightLimit = 10.0f;
    public float topLimit = 10.0f;
    public float bottomLimit = 0.0f;
    public float yOffset = 3.0f; // カメラのYオフセット

    public GameObject subScreen;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // プレイヤーの位置に基づいてカメラの座標を更新
            float x = player.transform.position.x;
            float y = player.transform.position.y + yOffset; // プレイヤーの5.0f下にカメラを置く
            float z = transform.position.z;

            // 左右の制限
            if (x < leftLimit)
            {
                x = leftLimit;
            }
            else if (x > rightLimit)
            {
                x = rightLimit;
            }

            // 上下の制限
            if (y < bottomLimit)
            {
                y = bottomLimit;
            }
            else if (y > topLimit)
            {
                y = topLimit;
            }

            Vector3 v3 = new Vector3(x, y, z);
            transform.position = v3;

            // サブスクリーンのスクロール処理
            if (subScreen != null)
            {
                float subScreenY = subScreen.transform.position.y;
                float subScreenZ = subScreen.transform.position.z;
                Vector3 v = new Vector3(x / 2.0f, subScreenY, subScreenZ);
                subScreen.transform.position = v;
            }
        }
    }
}
