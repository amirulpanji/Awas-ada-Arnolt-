using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;

    [Header("Hitbox Settings")]
    public Transform attackPoint;       // Objek penanda posisi pukulan (tangan/kaki)
    public float attackRange = 1.2f;    // Jangkauan panjang pukulan/tendangan
    public LayerMask enemyLayer;        // Pilih layer "Enemy" nanti di Inspector

    [Header("Damage Settings")]
    public int lightPunchDamage = 10;
    public int heavyKickDamage = 25;

    [Header("Attack Cooldown")]
    public float attackRate = 0.3f;     // Jeda waktu biar gak bisa spam tombol asal-asalan
    private float nextAttackTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Batasi serangan berdasarkan cooldown waktu
        if (Time.time >= nextAttackTime)
        {
            // 1. Tombol J untuk Pukulan Ringan (Light Punch)
            if (Input.GetKeyDown(KeyCode.J))
            {
                PerformAttack("punch", lightPunchDamage);
                nextAttackTime = Time.time + attackRate;
            }
            // 2. Tombol K untuk Tendangan Berat (Heavy Kick)
            else if (Input.GetKeyDown(KeyCode.K))
            {
                PerformAttack("kick", heavyKickDamage);
                nextAttackTime = Time.time + (attackRate + 0.15f); // Tendangan dikasih jeda lebih lama
            }
        }
    }

    void PerformAttack(string triggerName, int damage)
    {
        // 1. Picu animasi serangan di Animator
        if (anim != null)
        {
            anim.SetTrigger(triggerName);
        }

        if (attackPoint == null) return;

        // 2. Buat lingkaran invisible di attackPoint untuk mendeteksi apakah musuh kena hit
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // 3. Cek apakah ada objek ber-layer Enemy yang kena radius pukulan
        foreach (Collider2D enemy in hitEnemies)
        {   
            // Teks pembuktian di tab Console kalau serangan lo sukses masuk hit
            Debug.Log("BOOM! Serangan " + triggerName + " sukses mengenai " + enemy.name + " sebesar " + damage + " damage!");

            // TEMPAT MENARUH KODE DARAH MUSUH BERKURANG (Kita buat di tahap berikutnya):
            // enemy.GetComponent<EnemyHealth>().TakeDamage(damage);
        }
    }

    // Menggambar lingkaran panduan Hitbox berwarna cyan di jendela Scene
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}