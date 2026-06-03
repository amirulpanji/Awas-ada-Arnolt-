using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI Components")]
    public Slider healthSlider;        // Slider bar darah di Canvas
    public GameObject winScreenPanel; // Panel "Kamu Menang" (Hubungkan di Inspector MUSUH)
    public GameObject gameOverPanel;  // Panel "Game Over" (Hubungkan di Inspector PLAYER)

    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Setup tampilan awal slider darah jika dipasang
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        anim = GetComponent<Animator>();
    }

    // Fungsi untuk menerima damage dari pukulan/serangan
    public void TakeDamage(float damageAmount)
    {
        if (isDead) return; // Jika sudah mati, abaikan damage berikutnya

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Memastikan darah tidak minus

        // Update visual slider darah
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // Cek apakah darah sudah habis
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Memicu animasi berkedip/terluka jika ada parameternya di Animator
            if (anim != null) anim.SetTrigger("TakeDamage");
        }
    }

    // Fungsi internal saat karakter kehabisan darah
    void Die()
    {
        isDead = true;

        // Memicu animasi mati (Pastikan parameter 'IsDead' bertipe Bool sudah dibuat di Animator)
        if (anim != null) anim.SetBool("IsDead", true);

        // LINGKUP LOGIKA JIKA YANG MATI ADALAH PLAYER
        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("Player telah tewas!");

            // Matikan skrip pergerakan player agar tidak bisa digerakkan saat mati
            if (GetComponent<PlayerController>() != null)
                GetComponent<PlayerController>().enabled = false;

            // Munculkan UI kalah
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
        // LINGKUP LOGIKA JIKA YANG MATI ADALAH MUSUH (Semua Level)
        else
        {
            Debug.Log(gameObject.name + " telah dikalahkan!");

            // Matikan semua jenis AI musuh yang mungkin menempel pada objek ini
            if (GetComponent<FrogEnemyAI>() != null) GetComponent<FrogEnemyAI>().enabled = false;
            if (GetComponent<MaskedEnemyAI>() != null) GetComponent<MaskedEnemyAI>().enabled = false;
            if (GetComponent<RabbitClownAI>() != null) GetComponent<RabbitClownAI>().enabled = false;

            // Munculkan UI menang karena berhasil mengalahkan musuh
            if (winScreenPanel != null)
            {
                winScreenPanel.SetActive(true);
            }

            // Hancurkan jasad musuh dari arena setelah 2 detik agar tidak menumpuk
            Destroy(gameObject, 2f);
        }

        // Hentikan sisa momentum dorongan fisika saat karakter mati
        if (GetComponent<Rigidbody2D>() != null)
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        // Matikan collider agar jasad yang mati tidak menghalangi jalan karakter lain
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;
    }
}