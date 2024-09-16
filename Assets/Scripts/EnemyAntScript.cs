using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAntScript : MonoBehaviour
{
    public float Speed; // 敵の速度
    public float Angle; // 移動角度（度単位）
    Vector3 vec;

    //1フレーム当たりのStart関数を呼び出す
    void Start()
    {
        float rad = Angle * Mathf.Deg2Rad; // 角度をラジアンに変換
        vec = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0); // 移動方向のベクトルを計算
    }

    //1フレーム当たりのUpdate関数を呼び出す
    void Update()
    {
        // 毎フレームの移動を計算し、フレームレートに依存しないようにする
        transform.position += vec * Speed * Time.deltaTime;
    }
}