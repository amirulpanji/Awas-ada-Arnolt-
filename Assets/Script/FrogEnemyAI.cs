using UnityEngine;
using System.Collections;

public class FrogEnemyAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player; // Tarik objek Player ke sini nanti

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float chaseRange = 7f; // Jarak aman kodok mulai mengejar player
    public float attackRange = 1.5f; // Jarak untuk serangan biasa/jarak dekat

    [Header("Skill Settings")]
    public float skillCooldown = 4f; // Jeda waktu kodok bisa mengeluarkan skill lagi
    private bool canUseSkill = true;
    private bool isAttacking = false;

    [Header("Spit Attack Settings")]
    public GameObject spitPrefab; // Masukkan prefab cairan ijo di sini
    public Transform firePoint;   // Objek kosong di depan mulut kodok

    [Header("Leap Attack Damage Settings")]
    public float leapDamage = 30f;       // Damage lompatan kodok (cukup besar)
    public float leapDamageRadius = 1.5f;// Radius ledakan/hempasan saat mendarat
    public LayerMask playerLayer;


    // Komponen internal
    private Rigidbody2D rb;
    private Animator anim;
    private bool facingRight = false; // Biasanya sprite musuh menghadap kiri di awal

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Mencari player secara otomatis jika lupa ditarik di inspector
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        // Hitung jarak antara kodok dan player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Logika AI Berdasarkan Jarak
        if (distanceToPlayer <= attackRange)
        {
            // Jarak sangat dekat -> Stop jalan dan gunakan Skill Lompat/Smash
            StopMoving();
            if (canUseSkill)
            {
                StartCoroutine(LeapSmashSkill());
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // Jarak menengah -> Kejar player dan sesekali sembur racun
            ChasePlayer();

            if (canUseSkill && Random.value > 0.6f) // 40% peluang sembur sambil ngejar
            {
                StartCoroutine(FrogSpitterSkill());
            }
        }
        else
        {
            // Player terlalu jauh -> Kodok diam/kembali idle
            StopMoving();
        }

        // Atur arah hadap kodok agar selalu menghadap player
        LookAtPlayer();
    }

    void ChasePlayer()
    {
        anim.SetBool("IsRunning", true);

        // Tentukan arah menuju player (hanya horizontal X)
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
        if (player.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    // SKILL 1: Menyembur Racun (Jarak Menengah)
    IEnumerator FrogSpitterSkill()
    {
        isAttacking = true;
        canUseSkill = false;
        StopMoving();

        anim.SetTrigger("SpitAttack");

        // JEDA ANIMASI: Tunggu sampai frame mulut kodok terbuka (misal 0.3 detik)
        yield return new WaitForSeconds(0.3f);

        // Proses memunculkan cairan ijo
        if (spitPrefab != null && firePoint != null)
        {
            // Tentukan rotasi cairan berdasarkan arah hadap kodok
            // Jika kodok hadap kiri (skala X negatif), putar arah spawn-nya ke kiri (180 derajat)
            Quaternion spitRotation = transform.localScale.x < 0 ? Quaternion.Euler(0, 0, 180) : Quaternion.identity;

            Instantiate(spitPrefab, firePoint.position, spitRotation);
            Debug.Log("Manusia Kodok menyemburkan racun ijo!");
        }

        // Tunggu sisa animasi sampai benar-benar selesai sebelum bisa bergerak lagi
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;

        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true;
    }

    // SKILL 2: Melompat Hebat (Jarak Dekat)
    IEnumerator LeapSmashSkill()
    {
        isAttacking = true;
        canUseSkill = false;

        anim.SetTrigger("LeapAttack");
        Debug.Log("Manusia Kodok melakukan Leap Smash!");

        // Efek lompat ke arah player
        float leapDirection = (player.position.x > transform.position.x) ? 3f : -3f;
        rb.linearVelocity = new Vector2(leapDirection, 8f);

        // TUNGGU SAMPAI KODOK TURUN DAN MENDARAT (Misal 0.8 detik, sesuaikan dengan animasimu)
        yield return new WaitForSeconds(0.8f);

        // LOGIKA DAMAGE: Berikan damage hempasan saat kodok menginjak tanah
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, leapDamageRadius, playerLayer);
        if (hitPlayer != null)
        {
            Health playerHealth = hitPlayer.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(leapDamage);
                Debug.Log("Player terkena hempasan Leap Smash!");
            }
        }

        // Tunggu sisa waktu pemulihan animasi sebelum bisa bergerak lagi
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;

        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true;
    }

    // Menampilkan radius damage lompatan di jendela Scene (Warna Merah Putus-putus)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, leapDamageRadius);
    }
}
