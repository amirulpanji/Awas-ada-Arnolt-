using UnityEngine;

public class ClownBomb : MonoBehaviour
{
    public float explosionDamage = 25f;
    public float explosionRadius = 2f;
    public float fuseTime = 0.5f; // Jeda meledak setelah menyentuh sesuatu
    public LayerMask playerLayer;

    private bool hasTriggered = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Jika menyentuh lantai (Ground) atau menabrak Player, aktifkan sumbu bom
        if (!hasTriggered && (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Ground")))
        {
            hasTriggered = true;
            Invoke("Explode", fuseTime); // Panggil fungsi meledak setelah jeda fuseTime
        }
    }

    void Explode()
    {
        Debug.Log("BOOM! Bom Badut Meledak!");

        // Deteksi apakah player berada di dalam radius ledakan bom
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, explosionRadius, playerLayer);
        if (hitPlayer != null)
        {
            Health playerHealth = hitPlayer.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(explosionDamage);
            }
        }

        // BISA DITAMBAHKAN: Spawn partikel ledakan di sini kalau ada sprite ledakan

        Destroy(gameObject); // Hancurkan objek bom
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}