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
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        UpdateLevelUI();
    }

    void Update()
    {
        if (levelLoading) return;

        if (gun != null && gun.transform.position.z >= targetZ)
        {
            levelLoading = true;

            // Evolution kilidi kontrolü
            if (EvolutionState.Instance != null)
            {
                EvolutionState.Instance.UpdateUnlockProgressOnLevelComplete();
            }

            LoadNextLevel();
        }
    }

    void LoadNextLevel()
    {
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