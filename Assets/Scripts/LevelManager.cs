using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject gun;           // Assign your Gun object in the Inspector
    public float targetZ = 100f;     // The Z position to complete the level, since Gun is always going in one direction, a limit will decide when it should skip to the next level! 
    public TextMeshProUGUI levelText;           // Assign a UI Text element in the Inspector

    private int currentLevelIndex;

    void Start()
    {
        // Get active scene index
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        UpdateLevelUI();
    }

    void Update()
    {
        if (gun != null && gun.transform.position.z >= targetZ)
        {
            LoadNextLevel();
        }
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = currentLevelIndex + 1;
        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextLevelIndex);
        }
        // If last level, you may want to repeat, show a message, etc.?????
    }

    void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = $"Level {currentLevelIndex + 1}";
        }
    }
}