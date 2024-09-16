using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody;
    float axish = 0.0f;
    float lastDirection = 1.0f; // 直前の進行方向
    public float speed = 3.0f; // 通常の速度
    public float rotationSpeed = 10.0f; // PlayerRotation中の速度
    public float jump = 9.0f;
    public LayerMask groundLayer;
    int jumpCount = 0;
    public int maxJumpCount = 3;

    Animator animator;
    public string stopAnime = "PlayerStop";
    public string moveAnime = "PlayerMove";
    public string jumpAnime = "PlayerJump";
    public string attackAnime = "PlayerAttack";
    public string rotationAnime = "PlayerRotation";
    public string goalAnime = "PlayerGoal";
    public string deadAnime = "PlayerOver";
    string nowAnime = "";
    string oldAnime = "";

    public static string gameState = "playing";
    private bool canAttack = true; // 攻撃ができるかどうかのフラグ

    public int maxHP = 10;  // 最大HP
    private int currentHP; 
    private Coroutine hpReductionCoroutine;

    
    public Vector3 respawnPosition;  // 復活地点のポジション

    private bool PerformingAttack = false; // 攻撃中かどうかのフラグ
    
 private bool isTouchingEnemy = false; // エネミータグを持つものに当たっているかどうか
    public int CurrentHP
    {
        get { return currentHP; }
    }

    public AudioSource audioSource1;
    [SerializeField] AudioClip jumpSE;
    public AudioSource audioSource2;
    [SerializeField] AudioClip attacksuruSE;
    public AudioSource audioSource3;
    [SerializeField] AudioClip attackukeruSE;
    public AudioSource audioSource4;
    [SerializeField] AudioClip checkpointSE;
    public AudioSource audioSource5;
    [SerializeField] AudioClip rotationSE;
    public AudioSource audioSource6;
    [SerializeField] AudioClip healSE;

private bool canDetectEnemy = true; // エネミーを検知できるかどうかのフラグ

    void Start()
    {
        rbody = this.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        nowAnime = stopAnime;
        oldAnime = stopAnime;

        gameState = "playing";

        currentHP = maxHP;
        respawnPosition = transform.position;  // 初期位置を復活地点に設定

        audioSource1 = GetComponent<AudioSource>();
        audioSource2 = GetComponent<AudioSource>();
        audioSource3 = GetComponent<AudioSource>();
        audioSource4 = GetComponent<AudioSource>();
        audioSource5 = GetComponent<AudioSource>();
        audioSource6 = GetComponent<AudioSource>();

        

        // HP減少のコルーチンを開始
    hpReductionCoroutine = StartCoroutine(ReduceHPOverTime());
    }

    void Update()
    {
        if (gameState != "playing")
        {
            return;
        }

        axish = Input.GetAxisRaw("Horizontal");

        // 直前の進行方向を更新
        if (axish != 0.0f)
        {
            lastDirection = axish > 0.0f ? 1.0f : -1.0f;
        }

        // Sキーを押している間だけ、Shiftキーを押すことで攻撃を実行
        if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && canAttack)
            {
                audioSource5.PlayOneShot(rotationSE);
                nowAnime = attackAnime;
                StartCoroutine(PerformAttack()); // 攻撃処理を開始
                StartCoroutine(AttackCooldown()); // クールタイムを開始
            }
            else if (axish > 0.0f)
            {
                transform.localScale = new Vector2(1, 1);
                nowAnime = attackAnime;
            }
            else if (axish < 0.0f)
            {
                transform.localScale = new Vector2(-1, 1);
                nowAnime = attackAnime;
            }
            else
            {
                nowAnime = rotationAnime;
                speed = rotationSpeed; // 速度をrotationSpeedに設定
            }
        }
        else
        {
            speed = 3.0f; // 通常の速度に戻す
            // Sキーが押されていない場合は他のアニメーション処理
            if (axish > 0.0f)
            {
                transform.localScale = new Vector2(1, 1);
                nowAnime = moveAnime;
            }
            else if (axish < 0.0f)
            {
                transform.localScale = new Vector2(-1, 1);
                nowAnime = moveAnime;
            }
            else
            {
                nowAnime = stopAnime;
            }

            // 地面にいない場合はジャンプアニメーションに設定
            if (!Physics2D.CircleCast(transform.position, 0.1f, Vector2.down, 0.1f, groundLayer))
            {
                nowAnime = jumpAnime;
            }
        }

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            Jump();
        }

        if (nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime);
        }
    }

    void FixedUpdate()
    {
        if (gameState != "playing")
        {
            return;
        }

        bool onGround = Physics2D.CircleCast(transform.position, 0.1f, Vector2.down, 0.1f, groundLayer);

        if (onGround)
        {
            jumpCount = 0; // 地面に触れたときにジャンプ回数をリセット
        }

        // `PerformAttack` コルーチン中に速度を変更しているため、ここでの速度設定を調整
       // if (!PerformingAttack)
        //{
            //エネミーに当たっていない場合のみ移動処理を実行
        if (!isTouchingEnemy)
        {
            rbody.velocity = new Vector2(axish * speed, rbody.velocity.y);
        }
      // }
    }
     // エネミータグを持つオブジェクトに衝突したとき
    void OnCollisionEnter2D(Collision2D collision)
    {
         
       if (canDetectEnemy && collision.gameObject.CompareTag("Enemy"))
        {
            
            audioSource3.PlayOneShot(attackukeruSE);
            isTouchingEnemy = true;
            StartCoroutine(a3());
        }
    }
   private IEnumerator a3()
{
    canDetectEnemy = false; // 検知を無効化
    yield return new WaitForSeconds(0.3f); // 1秒間待機
    canDetectEnemy = true; // 検知を再び有効化
}

    // エネミータグを持つオブジェクトとの衝突が終了したとき
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
             StartCoroutine(ResetIsTouchingEnemyAfterDelay(0.5f)); // 1秒後にリセット
        }
    }
    // 1秒遅らせて isTouchingEnemy を false にするコルーチン
    IEnumerator ResetIsTouchingEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isTouchingEnemy = false;
    }

    public void Jump()
    {
        audioSource1.PlayOneShot(jumpSE);
        rbody.velocity = new Vector2(rbody.velocity.x, 0);
        Vector2 jumpPw = new Vector2(0, jump);
        rbody.AddForce(jumpPw, ForceMode2D.Impulse);
        jumpCount++;
    }

    private IEnumerator PerformAttack()
    {
        PerformingAttack = true; // 攻撃中フラグを設定

        // プレイヤーの現在の向きで攻撃方向を設定
        float direction = lastDirection; // 直前の進行方向を使用
        Vector2 attackDirection = new Vector2(direction, 0); // 攻撃方向を設定

        // 0.5秒間移動を停止
        rbody.velocity = new Vector2(0, rbody.velocity.y);
        yield return new WaitForSeconds(0.1f);

        // 1.5秒間速さを100.0fに設定
        float originalSpeed = speed; // 元の速度を保存
        speed = 20.0f;
        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
      {
    elapsed += Time.deltaTime;
    rbody.velocity = new Vector2(direction * speed, rbody.velocity.y);

    // 攻撃中にエネミーを検知して吹っ飛ばす
    Vector2 circleCenter = (Vector2)transform.position + Vector2.up * 0.5f; // 上に0.5fのオフセットを加える
    float blowbackRadius = 1.0f; // 同心円の半径
    Collider2D[] enemies = Physics2D.OverlapCircleAll(circleCenter, blowbackRadius, LayerMask.GetMask("Enemy"));
    
    foreach (Collider2D enemy in enemies)
    {
        if (enemy != null && enemy.CompareTag("Enemy"))
        {
            
            Debug.Log("Enemy Hit");
            Rigidbody2D enemyRbody = enemy.GetComponent<Rigidbody2D>();
            if (enemyRbody != null)
            {
                audioSource2.PlayOneShot(attacksuruSE); // 音を再生
                Vector2 directionToEnemy = (enemy.transform.position - transform.position).normalized;
                Vector2 blowback = directionToEnemy * 70.0f; // 吹っ飛ばす方向と力
                blowback.y = 10.0f; // 上方向の力
                enemyRbody.AddForce(blowback, ForceMode2D.Impulse);
            }
             // 0.5秒待機してから次の敵へ
            yield return new WaitForSeconds(2f);
        }
    }
    PerformingAttack = false; // 攻撃中フラグを解除
    yield return null;
      }


        // 速度を元に戻す
        speed = originalSpeed;
        PerformingAttack = false; // 攻撃中フラグを解除
    }



    private IEnumerator AttackCooldown()
    {
        canAttack = false; // 攻撃を無効にする
        yield return new WaitForSeconds(1.0f); // 1秒のクールタイム
        canAttack = true; // 攻撃を再び有効にする
    }

    public   void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Goal();
        }
        else if (collision.gameObject.tag == "Dead")
        {
            GameOver();
        }
        else if (collision.gameObject.tag == "Item")
        {
         
        Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "HealItem")
        {
            audioSource6.PlayOneShot(healSE);
            currentHP +=3;
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "HealItem2")
        {
             audioSource6.PlayOneShot(healSE);
            currentHP +=5;
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "HealItem3")
        {
            audioSource6.PlayOneShot(healSE);
            currentHP +=10;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Checkpoint")  // 中間ゴール地点
        {
            audioSource4.PlayOneShot(checkpointSE);
            Debug.Log("Checkpoint reached! Setting respawn position.");  // ログを追加
            respawnPosition = transform.position;  // 現在位置を復活地点に設定
            currentHP +=10 ;  // HPを全回復
            GameManager.instance.UpdateHearts(currentHP);  // ハートUIの更新
            collision.gameObject.SetActive(false);  // 一度触れたら非表示にする
        }
    }

    public void Goal()
    {
        animator.Play(goalAnime);
        gameState = "gameclear";
        GameStop();
        // HP減少のコルーチンを停止
        StopCoroutine("ReduceHPOverTime");
    }

    public void GameOver()
    {
        animator.Play(deadAnime);
        gameState = "gameover";
        GameStop();

        // ゲームマネージャーのGameOverメソッドを呼び出す
    if (GameManager.instance != null)
    {
        Debug.Log("GameOver method called.");  // ログを追加
      //  GameManager.instance.GameOver();

    }
    }

 public   void GameStop()
    {
        Rigidbody2D rbody = GetComponent<Rigidbody2D>();
        rbody.velocity = new Vector2(0, 0);
         // HP減少のコルーチンを停止
    if (hpReductionCoroutine != null)
    {
        StopCoroutine(hpReductionCoroutine);
        hpReductionCoroutine = null; // コルーチンが停止されたらnullにする
    }
    } 

    private IEnumerator ReduceHPOverTime()
    {
        while (currentHP > 0)
        {
            currentHP -= 1;  // 修正後
            Debug.Log("HP: " + currentHP); // HPのログを出力
            yield return new WaitForSeconds(1.0f); // 1秒ごとにHPを減らす
        }
        
        if (currentHP <= 0)
        {
            gameState = "gameover"; // HPが0以下になったらゲームオーバーにする
            Debug.Log("Game Over");
        }
    }

    public void Respawn()
{
    Debug.Log("Respawning at position: " + respawnPosition);  // デバッグログ
    transform.position = respawnPosition;  // 最新の復活地点に移動
    currentHP = maxHP;  // HPを全回復
    GameManager.instance.UpdateHearts(currentHP);  // ハートUIの更新
    GetComponent<BoxCollider2D>().enabled = true;  // コライダーを有効にする
    rbody.velocity = Vector2.zero;  // 動きをリセット
    animator.Play(stopAnime);  // 復活時のアニメーションを設定
    gameState = "playing";  // ゲームステートをプレイ中に戻す
    hpReductionCoroutine = StartCoroutine(ReduceHPOverTime());
}
}
