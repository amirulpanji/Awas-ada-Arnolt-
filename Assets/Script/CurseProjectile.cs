using UnityEngine;
using System.Collections;

public class CurseProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float damage = 5f;
    public float slowDuration = 3f;
    public float lifeTime = 4f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            PlayerController playerCtrl = collision.GetComponent<PlayerController>();

            if (playerHealth != null) playerHealth.TakeDamage(damage);

            // Berikan efek slow ke player jika skrip PlayerController ditemukan
            if (playerCtrl != null)
            {
                playerCtrl.StartCoroutine(ApplySlowEffect(playerCtrl));
            }

            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }

    // Coroutine untuk memperlambat player lalu mengembalikannya ke normal
    IEnumerator ApplySlowEffect(PlayerController player)
    {
        float originalSpeed = player.moveSpeed;
        player.moveSpeed = originalSpeed * 0.5f; // Kurangi kecepatan jadi 50%
        Debug.Log("Player terkena kutukan! Gerakan melambat.");

        yield return new WaitForSeconds(slowDuration);

        player.moveSpeed = originalSpeed; // Kembalikan ke normal
        Debug.Log("Efek kutukan suku bertopeng telah hilang.");
    }
}