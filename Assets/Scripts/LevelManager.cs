using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject gun;           // Assign your Gun object in the Inspector
    public float targetZ = 2000f;     // The Z position to complete the level, since Gun is always going in one direction, a limit will decide when it should skip to the next level! 
    // IMPORTANT: change this value from "LevelIndexCanvas" component directly!!!! 
    public TextMeshProUGUI levelText;           // Assign a UI Text element in the Inspector

    private int currentLevelIndex;
    private bool levelLoading = false; // Seviye yüklemesinin birden çok kez tetiklenmesini önler

    void Start()
    {
        // Get active scene index
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        UpdateLevelUI();
    }

    void Update()
    {
        // Seviye zaten yükleniyorsa tekrar kontrol etme
        if (levelLoading) return;

        // Seviye tamamlama koþulu: Sadece silah hedef Z pozisyonuna ulaþtýysa
        if (gun != null && gun.transform.position.z >= targetZ)
        {
            levelLoading = true; // Yüklemeyi baþlat ve bayraðý ayarla
            LoadNextLevel();
        }
    }

    void LoadNextLevel()
    {
        // Sonraki seviyeye geçmeden önce mevcut seviyenin buildIndex'ini kaydet.
        if (GameStateManager.Instance != null)
        {
            int completedLevelIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log($"[LevelManager] Seviye tamamlandý (Mesafe). Kaydedilen buildIndex: {completedLevelIndex}");
            GameStateManager.Instance.LastCompletedLevelIndex = completedLevelIndex;
        }
        else
        {
            Debug.LogError("[LevelManager] GameStateManager örneði bulunamadý! Seviye indeksi kaydedilemedi.");
        }

        // Instead of loading next level, go to weapon merge scene
        // The merge scene will handle returning to the game
        SceneManager.LoadScene("weaponMerge");
    }

    void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = $"Level {currentLevelIndex + 1}";
        }
    }
}