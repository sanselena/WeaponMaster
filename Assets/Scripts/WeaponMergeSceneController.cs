using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class WeaponMergeSceneController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject presentRevealPanel; 
    public GameObject mergeTablePanel;    

    [Header("Present Reveal Settings")]
    public GameObject presentBoxPrefab;    
    public Transform presentRevealPosition;  
    public float presentRevealScale = 3f;
    public float revealAnimationDuration = 1f;

    [Header("Evolution")]
    public Button evolutionButton;

    private GameObject presentInstance;
    private bool hasRevealedPresent = false;

    void Start()
    {
        if (Time.timeScale != 1f)
        {
            Debug.LogWarning($"Time.timeScale was {Time.timeScale}. Setting it back to 1f.");
            Time.timeScale = 1f;
        }

        if (GameStateManager.Instance != null && GameStateManager.Instance.HasPendingPresent)
        {
            StartCoroutine(ShowPresentRevealFlow());
        }
        else
        {
            ShowMergeTableOnly();
        }
    }

    private IEnumerator ShowPresentRevealFlow()
    {
        if (presentRevealPanel != null) presentRevealPanel.SetActive(true);
        if (mergeTablePanel != null) mergeTablePanel.SetActive(false);

        if (evolutionButton != null) evolutionButton.gameObject.SetActive(false);

        if (presentBoxPrefab != null && presentRevealPosition != null)
        {
            presentInstance = Instantiate(presentBoxPrefab, presentRevealPosition.position, presentRevealPosition.rotation);
            presentInstance.transform.localScale = Vector3.one * presentRevealScale;
        }

        hasRevealedPresent = false;
        yield return null;
    }

    public void OnPresentBoxClicked()
    {
        if (hasRevealedPresent) return;
        hasRevealedPresent = true;
        StartCoroutine(RevealAnimation());
    }

    private IEnumerator RevealAnimation()
    {
        if (presentInstance != null)
        {
            float elapsed = 0f;
            Vector3 startScale = presentInstance.transform.localScale;
            while (elapsed < revealAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / revealAnimationDuration;
                presentInstance.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, progress);
                presentInstance.transform.Rotate(Vector3.up, 720f * Time.deltaTime);
                yield return null;
            }
            Destroy(presentInstance);
        }

        yield return new WaitForSeconds(0.5f);
        ShowMergeTableOnly();
    }

    private void ShowMergeTableOnly()
    {
        if (presentRevealPanel != null) presentRevealPanel.SetActive(false);
        if (mergeTablePanel != null) mergeTablePanel.SetActive(true);

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.HasPendingPresent = false;
        }

        SetupEvolutionButton();

        var gridManager = Object.FindAnyObjectByType<WeaponPartGridManager>();
        if (gridManager != null)
        {
            gridManager.RefreshGrid();
        }
    }

    private void SetupEvolutionButton()
    {
        if (evolutionButton == null) return;

        bool unlocked = EvolutionState.Instance != null && EvolutionState.Instance.EvolutionUnlocked;
        bool isBase   = EvolutionState.Instance == null || EvolutionState.Instance.CurrentGunVariant == GunVariant.Base;
        bool canShow  = unlocked && isBase;

        Debug.Log($"[WeaponMergeSceneController] EvolutionButton visible? {canShow} (unlocked={unlocked}, isBase={isBase})");

        evolutionButton.gameObject.SetActive(canShow);

        evolutionButton.onClick.RemoveListener(OnEvolutionClicked);
        if (canShow)
        {
            evolutionButton.onClick.AddListener(OnEvolutionClicked);
        }
    }

    private void OnEvolutionClicked()
    {
        if (EvolutionState.Instance != null)
        {
            EvolutionState.Instance.ApplyEvolution();
        }

        var switcher = Object.FindAnyObjectByType<GunModelSwitcher>();
        if (switcher != null) switcher.RefreshModel();

        var shooter = Object.FindAnyObjectByType<GunShooting>();
        if (shooter != null) shooter.ReapplyVariantBaseStatsAndBonus();

        var gridManager = Object.FindAnyObjectByType<WeaponPartGridManager>();
        if (gridManager != null)
        {
            gridManager.RefreshGrid();
        }

        if (evolutionButton != null)
        {
            evolutionButton.onClick.RemoveListener(OnEvolutionClicked);
            evolutionButton.gameObject.SetActive(false);
        }
    }

    public void ReturnToGame()
    {
        int nextLevelIndex = 0;
        if (GameStateManager.Instance != null)
        {
            int lastCompletedIndex = GameStateManager.Instance.LastCompletedLevelIndex;
            Debug.Log($"[WeaponMergeSceneController] GameStateManager'den okunan LastCompletedLevelIndex: {lastCompletedIndex}");
            nextLevelIndex = (lastCompletedIndex == -1) ? 0 : lastCompletedIndex + 1;
        }
        else
        {
            Debug.LogError("[WeaponMergeSceneController] GameStateManager örneði bulunamadý! Varsayýlan olarak seviye 0 yüklenecek.");
        }

        Debug.Log($"[WeaponMergeSceneController] Yüklenecek sonraki seviye indeksi: {nextLevelIndex}");

        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextLevelIndex);
        }
        else
        {
            Debug.LogWarning($"Sonraki seviye (index: {nextLevelIndex}) bulunamadý. Seviye 0'a dönülüyor.");
            SceneManager.LoadScene(0);
        }
    }
}

