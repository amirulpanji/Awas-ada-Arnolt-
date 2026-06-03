using UnityEngine;
using System.Collections;

public class RabbitClownAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;        // Sangat lincah dan cepat
    public float chaseRange = 10f;
    public float attackRange = 2.5f;    // Jarak untuk memulai serangan putar

    [Header("Skill Settings")]
    public float skillCooldown = 4f;
    public GameObject bombPrefab;       // Prefab bom badut
    public Transform throwPoint;        // Titik tangan saat melempar bom

    [Header("Spin Attack Settings")]
    public float spinDamage = 8f;       // Damage per detik saat berputar
    public float spinDuration = 2f;     // Durasi berputar

    private bool canUseSkill = true;
    private bool isAttacking = false;
    private bool isSpinning = false;
    private Rigidbody2D rb;
    private Animator anim;
    private bool facingRight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // LOGIKA AI BADUT KELINCI
        if (distanceToPlayer <= attackRange)
        {
            if (canUseSkill)
            {
                StartCoroutine(SpinAttackSkill());
            }
            else
            {
                StopMoving();
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // Jika bisa pake skill dari jauh, lempar bom. Jika tidak, kejar player.
            if (canUseSkill && Random.value > 0.6f)
            {
                StartCoroutine(ThrowBombSkill());
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            StopMoving();
        }

        // Jangan flip arah hadap kalau lagi muter-muter gila
        if (!isSpinning)
        {
            LookAtPlayer();
        }
    }

    void ChasePlayer()
    {
        anim.SetBool("IsRunning", true);
        float direction = (player.position.x > transform.position.x) ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    void StopMoving()
    {
        anim.SetBool("IsRunning", false);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void LookAtPlayer()
    {
        if (player.position.x > transform.position.x && !facingRight) Flip();
        else if (player.position.x < transform.position.x && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    // SKILL 1: Melempar Bom Badut (Jarak Jauh)
    IEnumerator ThrowBombSkill()
    {
        isAttacking = true;
        canUseSkill = false;
        StopMoving();

        anim.SetTrigger("Throw");
        yield return new WaitForSeconds(0.3f); // Jeda frame lempar

        if (bombPrefab != null && throwPoint != null)
        {
            GameObject projectile = Instantiate(bombPrefab, throwPoint.position, Quaternion.identity);

            // Memberikan efek lemparan melengkung (parabola) menggunakan Rigidbody
            Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
            if (projRb != null)
            {
                float throwDirection = facingRight ? 1f : -1f;
                // Dorong ke depan (X) dan sedikit ke atas (Y)
                projRb.linearVelocity = new Vector2(throwDirection * 6f, 8f);
            }
        }

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;

        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true;
    }

    // SKILL 2: Serangan Putar Gila (Jarak Dekat)
    IEnumerator SpinAttackSkill()
    {
        isAttacking = true;
        canUseSkill = false;
        isSpinning = true;

        anim.SetTrigger("Spin");
        Debug.Log("Badut Kelinci Berputar Gila!");

        float timer = 0f;
        float spinDirection = (player.position.x > transform.position.x) ? 1.5f : -1.5f;

        // Selama durasi spin, badut akan bergerak maju menerjang player secara brutal
        while (timer < spinDuration)
        {
            rb.linearVelocity = new Vector2(spinDirection * moveSpeed, rb.linearVelocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        StopMoving();
        isSpinning = false;
        yield return new WaitForSeconds(0.5f); // Jeda pusing setelah berputar
        isAttacking = false;

        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true;
    }

    // Memicu damage berkali-kali jika player bersentuhan saat badut sedang berputar
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isSpinning && collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Menghasilkan damage konstan per frame (dibagi waktu)
                playerHealth.TakeDamage(spinDamage * Time.deltaTime * 10f);
            }
        }
    }
}