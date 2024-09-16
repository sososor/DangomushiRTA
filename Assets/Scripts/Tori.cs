using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tori : MonoBehaviour
{
    public float speed = 2.0f; // 鳥の移動速度
    public float moveDistance = 5.0f; // 移動する距離

    private Vector3 startPosition; // 初期位置
    private bool movingRight = true; // 進行方向

    private Rigidbody2D rbody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public string MoveAnime = "ToriMove"; // アニメーション名

    // Start is called before the first frame update
    void Start()
    {
        // 鳥の初期位置を保存
        startPosition = transform.position;
        rbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRendererを取得
    }

    // Update is called once per frame
    void Update()
    {
        // 鳥を移動させる
        MoveBird();
    }

    // 鳥の移動を管理する
    void MoveBird()
    {
        // 移動距離を計算
        float distanceMoved = Vector3.Distance(transform.position, startPosition);

        // 指定した距離に達したら進行方向を反転
        if (distanceMoved >= moveDistance)
        {
            movingRight = !movingRight; // 進行方向を反転
            startPosition = transform.position; // 新しいスタート地点を設定

            // アニメーションの反転
            spriteRenderer.flipX = !movingRight; // 右に進むときはそのまま、左に進むときは反転
        }

        // 鳥の移動
        if (movingRight)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }

        // アニメーションを再生
        if (animator != null)
        {
            animator.Play(MoveAnime);
        }
    }

     void OnCollisionEnter2D(Collision2D collision)
{
    Debug.Log("Collision detected with: " + collision.gameObject.name);

    if (collision.gameObject.CompareTag("Player"))
    {
        Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

        if (playerRigidbody != null)
        {
            Debug.Log("Applying force to player");

            // X軸とY軸の力を加える
            Vector2 force = new Vector2(-70.0f, 5.0f);
            playerRigidbody.AddForce(force, ForceMode2D.Impulse);

            // 力を加えた後の速度をログに出力
            Debug.Log("Player velocity after force: " + playerRigidbody.velocity);
        }
    }
}
}
