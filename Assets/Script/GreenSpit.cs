using UnityEngine;

public class GreenSpit : MonoBehaviour
{
    public float speed = 8f;
    public float damage = 10f;
    public float lifeTime = 3f; // Hancur otomatis setelah 3 detik jika tidak kena apa-apa

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Membuat cairan meluncur ke arah depan (sesuai arah rotasi/skala spawn)
        rb.linearVelocity = transform.right * speed;

        // Hancurkan objek setelah beberapa detik agar tidak memenuhi memori game
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Mencari komponen Health yang ada di tubuh Player
            Health playerHealth = collision.GetComponent<Health>();

            if (playerHealth != null)
            {
                // Kurangi nyawa player sesuai nilai damage cairan
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject); // Cairan ijo menghilang
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}