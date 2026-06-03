using UnityEngine;
using UnityEngine.SceneManagement; // Wajib untuk memuat scene level

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelPanel;
    public GameObject helpPanel;

    void Start()
    {
        // Memastikan saat game dibuka, hanya menu utama yang kelihatan
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (levelPanel != null) levelPanel.SetActive(false);
        if (helpPanel != null) helpPanel.SetActive(false);
    }

    // ================= TOMBOL UTAMA =================

    // 1. Fungsi saat tombol PLAY diklik
    public void OpenLevelPanel()
    {
        levelPanel.SetActive(true);
        mainMenuPanel.SetActive(false); // Sembunyikan menu utama agar rapi
    }

    // 2. Fungsi saat tombol HELP diklik
    public void OpenHelpPanel()
    {
        helpPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    // 3. Fungsi saat tombol SETTINGS diklik (Bisa dikembangkan nanti)
    public void OpenSettings()
    {
        Debug.Log("Membuka menu pengaturan... (Fitur ini bisa kamu tambah nanti)");
    }

    // ================= TOMBOL KEMBALI (BACK) =================

    public void CloseLevelPanel()
    {
        levelPanel.SetActive(false);
        mainMenuPanel.SetActive(true); // Munculkan kembali menu utama
    }

    public void CloseHelpPanel()
    {
        helpPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // ================= TOMBOL PEMILIHAN LEVEL =================

    // Fungsi serbaguna untuk masuk ke level berdasarkan nama scene
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    // Fungsi untuk keluar dari game (Hanya bekerja setelah game di-build/export)
    public void QuitGame()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();
    }
}