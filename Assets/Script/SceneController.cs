using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Pause Menu Settings")]
    public GameObject pauseMenuPanel; // Tarik panel pause ke sini di Inspector
    public static bool isPaused = false; // Status untuk mengecek apakah game sedang berhenti

    void Update()
    {
        // Mendeteksi jika player menekan tombol Escape (ESC)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // 1. Fungsi untuk Menghentikan Game
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true); // Munculkan UI Pause
        Time.timeScale = 0f;            // Menghentikan waktu game (animasi & fisika berhenti)
        isPaused = true;
    }

    // 2. Fungsi untuk Melanjutkan Game
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false); // Sembunyikan UI Pause
        Time.timeScale = 1f;             // Mengembalikan waktu game ke normal
        isPaused = false;
    }

    // 3. Fungsi untuk Mengulang Level
    public void RestartLevel()
    {
        Time.timeScale = 1f; // WAJIB: Kembalikan waktu ke normal sebelum reload scene!
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 4. Fungsi untuk Pindah Scene berdasarkan Nama
    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f; // WAJIB: Kembalikan waktu ke normal sebelum pindah level!
        isPaused = false;
        SceneManager.LoadScene(sceneName);
    }
}