using UnityEngine;

public class FightingPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 14f;

    [Header("Target Lock")]
    private Transform opponentTarget;

    private Rigidbody2D rb;
    private Animator anim;
    private float horizontalInput;
    private bool isGrounded;

    [Header("Ground Check (Raycast Method)")]
    public LayerMask groundLayer;      // Pilih layer "Ground" di Inspector
    public float raycastDistance = 1.3f; // Panjang sinar laser ke bawah kaki

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // SELIPIN KODE INI UNTUK TES INPUT:
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Tombol D ditekan!");
        }

        // (Sisa kode input yang lama di bawahnya biarkan saja...)
        horizontalInput = Input.GetAxisRaw("Horizontal");
        // ...
    }

    void FixedUpdate()
    {
        // Jalankan pergerakan fisik
        if (horizontalInput != 0)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        // DETEKSI TANAH BARU: Menembakkan laser dari tengah badan Player lurus ke bawah
        // Jika laser menabrak layer Ground, maka isGrounded = true
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);

        if (hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void UpdateAnimations()
    {
        if (anim == null) return;

        // Mengirim data ke Animator
        anim.SetFloat("speed", Mathf.Abs(horizontalInput));
        anim.SetBool("isGrounded", isGrounded);
    }

    void AutoFlipTowardsOpponent()
    {
        if (opponentTarget == null)
        {
            GameObject enemyObj = GameObject.FindWithTag("Enemy");
            if (enemyObj != null) opponentTarget = enemyObj.transform;
        }

        if (opponentTarget == null) return;

        if (opponentTarget.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (opponentTarget.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Menggambar garis laser panduan di jendela Scene biar lo bisa pasin panjangnya
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * raycastDistance);
    }
}