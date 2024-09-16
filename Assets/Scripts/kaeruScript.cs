using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kaeruScript : MonoBehaviour
{
    Rigidbody2D rbody;
    public float moveSpeed = 2.0f; // 前に進む速度
    public float jumpForce = 9.0f; // ジャンプの力
    public float jumpInterval = 2.0f; // ジャンプの周期（秒）

    Animator animator;
    public string stopAnime = "kaeruStop";
    public string jumpAnime = "kaeruJump";

    public LayerMask groundLayer; // 地面を判定するレイヤー
    public float groundCheckRadius = 0.1f; // 地面判定の半径

    private bool isJumping = false;
    private bool onGround = false;
    private float groundedTime = 0.0f; // 地面にいる時間を計測

    public float requiredGroundedTime = 3.0f; // 地面に何秒いるとジャンプするかの時間


    // Start is called before the first frame update
    void Start()
    {
        rbody = this.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Jump()
{
    // 現在の横方向の速度を取得
    float horizontalVelocity = rbody.velocity.x;
    
    // ジャンプする際に、横方向の速度も考慮して、上方向に力を加える
    Vector2 jumpPw = new Vector2(moveSpeed, jumpForce);
    rbody.velocity = jumpPw; // 横方向と上方向の速度を同時に設定

    // ジャンプアニメーションを再生
    animator.Play(jumpAnime); 
}


    // FixedUpdate で物理演算を扱う
    void FixedUpdate()
    {
        // Physics2D.CircleCast を使って地面判定を行う
        onGround = Physics2D.CircleCast(transform.position, groundCheckRadius, Vector2.down, 0.1f, groundLayer);

        // デバッグ用にCircleCastを可視化
        //Debug.DrawRay(transform.position, Vector2.down * 0.1f, Color.red);

        // 地面にいる場合のアニメーション制御
        if (onGround)
        {
            groundedTime += Time.fixedDeltaTime;
            if (groundedTime < requiredGroundedTime)
            {
                // 地面にいる間は止まっているアニメーションを再生
                animator.Play(stopAnime);
            }
            else if (groundedTime >= requiredGroundedTime)
            {
                Jump();
                groundedTime = 0.0f; // ジャンプしたらカウンターをリセット
            }
        }
        else
        {
            // 空中にいる間はカウンターをリセット
            groundedTime = 0.0f;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
{
 //   Debug.Log("Collision detected with: " + collision.gameObject.name);

    if (collision.gameObject.CompareTag("Player"))
    {
        Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

        if (playerRigidbody != null)
        {
          //  Debug.Log("Applying force to player");

            // X軸とY軸の力を加える
            Vector2 force = new Vector2(-70.0f, 5.0f);
            playerRigidbody.AddForce(force, ForceMode2D.Impulse);

            // 力を加えた後の速度をログに出力
          //  Debug.Log("Player velocity after force: " + playerRigidbody.velocity);
        }
    }
}

}
