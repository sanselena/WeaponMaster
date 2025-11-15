using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GamePauseManager : MonoBehaviour
{
    public static GamePauseManager Instance { get; private set; }

    private bool isPaused = false;
    private GunMovement gunMovement;
    private GunShooting gunShooting;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Find gun components
        GameObject gun = GameObject.FindGameObjectWithTag("Player");
        if (gun != null)
        {
            gunMovement = gun.GetComponent<GunMovement>();
            gunShooting = gun.GetComponent<GunShooting>();
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f; // Freeze time

        // Disable gun movement
        if (gunMovement != null)
        {
            gunMovement.enabled = false;
        }

        // Disable gun shooting
        if (gunShooting != null)
        {
            gunShooting.enabled = false;
        }

        Debug.Log("Game paused");
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f; // Resume time

        // Enable gun movement
        if (gunMovement != null)
        {
            gunMovement.enabled = true;
        }

        // Enable gun shooting
        if (gunShooting != null)
        {
            gunShooting.enabled = true;
        }

        Debug.Log("Game resumed");
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    // Bu metod, oyunu duraklatýr ve belirtilen süre sonra yeni sahneyi yükler.
    public void PauseAndLoadScene(string sceneName, float delay)
    {
        StartCoroutine(PauseAndLoadRoutine(sceneName, delay));
    }

    private IEnumerator PauseAndLoadRoutine(string sceneName, float delay)
    {
        // Oyunu tamamen durdur
        Time.timeScale = 0f;

        // Gerçek zamanla bekle (Time.timeScale'den etkilenmez)
        yield return new WaitForSecondsRealtime(delay);

        // Zamaný normale döndür
        Time.timeScale = 1f;

        // Yeni sahneyi yükle
        SceneManager.LoadScene(sceneName);
    }
}

