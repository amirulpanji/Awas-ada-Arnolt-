using UnityEngine;
using TMPro; // Wajib jika memakai TextMeshPro, ganti dengan UnityEngine.UI jika pakai Text biasa
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueLine
    {
        public string characterName;  // Siapa yang bicara (Player / Kodok / Badut)
        [TextArea(2, 5)]
        public string sentence;       // Apa kalimatnya
    }

    [Header("UI Components")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    [Header("Dialogue Content")]
    public List<DialogueLine> dialogueLines; // Isi dialog yang bisa kita ketik di Inspector

    [Header("References to Freeze")]
    public PlayerController playerController;
    public MonoBehaviour enemyAI; // Bisa diisi FrogEnemyAI, MaskedEnemyAI, atau RabbitClownAI

    private Queue<DialogueLine> linesQueue; // Antrean kalimat

    void Start()
    {
        linesQueue = new Queue<DialogueLine>();
        StartDialogue();
    }

    // 1. Memulai Dialog di Awal Level
    public void StartDialogue()
    {
        // Matikan kontroler biar gak bisa gerak/tarung pas ngobrol
        if (playerController != null) playerController.enabled = false;
        if (enemyAI != null) enemyAI.enabled = false;

        dialoguePanel.SetActive(true);
        linesQueue.Clear();

        // Masukkan semua baris kalimat ke dalam antrean (Queue)
        foreach (DialogueLine line in dialogueLines)
        {
            linesQueue.Enqueue(line);
        }

        DisplayNextSentence();
    }

    // 2. Memunculkan Kalimat Berikutnya saat Tombol NEXT Diklik
    public void DisplayNextSentence()
    {
        // Jika antrean kalimat sudah habis, akhiri dialog
        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Ambil kalimat paling depan di antrean
        DialogueLine currentLine = linesQueue.Dequeue();

        // Update teks di UI
        nameText.text = currentLine.characterName;
        dialogueText.text = currentLine.sentence;
    }

    // 3. Menutup Kotak Dialog dan Memulai Pertarungan
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        // Aktifkan kembali kontroler agar Player dan Musuh bisa baku hantam!
        if (playerController != null) playerController.enabled = true;
        if (enemyAI != null) enemyAI.enabled = true;

        Debug.Log("Dialog selesai! FIGHT!");
    }
}