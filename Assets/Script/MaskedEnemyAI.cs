using UnityEngine;
using System.Collections;

public class MaskedEnemyAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;

    [Header("Movement Settings")]
    public float moveSpeed = 4f;       // Lebih lincah dari kodok
    public float chaseRange = 8f;
    public float attackRange = 2f;     // Jarak tebasan setelah teleport

    [Header("Skill Settings")]
    public float skillCooldown = 5f;
    public float teleportBehindOffset = 1.5f; // Jarak kemunculan di belakang player
    public GameObject cursePrefab;     // Prefab asap kutukan
    public Transform staffPoint;       // Titik keluar kutukan

    private bool canUseSkill = true;
    private bool isAttacking = false;
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

        // LOGIKA AI SUKU BERTOPENG
        if (distanceToPlayer <= attackRange)
        {
            StopMoving();
            if (canUseSkill)
            {
                StartCoroutine(TeleportStrikeSkill());
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // Suku bertopeng suka menjaga jarak, jika canUseSkill aktif dari jauh, dia lempar kutukan dulu
            if (canUseSkill && Random.value > 0.5f)
            {
                StartCoroutine(CastCurseSkill());
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

        LookAtPlayer();
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

    // SKILL 1: Melempar Kutukan Jarak Jauh (Memperlambat Player)
    IEnumerator CastCurseSkill()
    {
        isAttacking = true;
        canUseSkill = false;
        StopMoving();

        anim.SetTrigger("CastSpell");
        yield return new WaitForSeconds(0.4f); // Tunggu animasi angkat tongkat/tangan

        if (cursePrefab != null && staffPoint != null)
        {
            Quaternion spawnRotation = transform.localScale.x < 0 ? Quaternion.Euler(0, 0, 180) : Quaternion.identity;
            Instantiate(cursePrefab, staffPoint.position, spawnRotation);
            Debug.Log("Suku Bertopeng melemparkan kutukan asap!");
        }

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;

        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true;
    }

    // SKILL 2: Teleportasi Instan ke Belakang Player + Menebas
    IEnumerator TeleportStrikeSkill()
    {
        isAttacking = true;
        canUseSkill = false;

        anim.SetTrigger("Teleport");
        Debug.Log("Suku Bertopeng menghilang...");

        // Efek menghilang (matikan collider biar ga bisa dipukul saat teleport)
        GetComponent<Collider2D>().enabled = false;
        rb.simulated = false;

        yield return new WaitForSeconds(0.5f); // Jeda durasi menghilang

        // Hitung posisi di belakang Player berdasarkan arah hadap Player
        float playerDirection = player.localScale.x > 0 ? -1f : 1f;
        Vector3 teleportPos = new Vector3(player.position.x + (playerDirection * teleportBehindOffset), transform.position.y, transform.position.z);

        // Pindah posisi instan
        transform.position = teleportPos;

        // Muncul kembali
        GetComponent<Collider2D>().enabled = true;
        rb.simulated = true;
        Debug.Log("Suku Bertopeng muncul di belakang Player dan menebas!");

        // Berikan damage tebakan instan jika player tidak menghindar
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null) playerHealth.TakeDamage(20f); // Damage tebasan
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;

        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true;
    }
}