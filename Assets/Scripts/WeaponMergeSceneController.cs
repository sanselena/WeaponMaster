using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WeaponMergeSceneController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject presentRevealPanel; // Hediye gösterme panelini Inspector'dan buraya sürükle
    public GameObject mergeTablePanel;    // Birleþtirme masasý panelini Inspector'dan buraya sürükle

    [Header("Present Reveal Settings")]
    public GameObject presentBoxPrefab;      // 'presentBox' prefab'ýný Project panelinden buraya sürükle
    public Transform presentRevealPosition;  // Hiyerarþideki 'presentRevealPosition' objesini buraya sürükle
    public float presentRevealScale = 3f;
    public float revealAnimationDuration = 1f;

    private GameObject presentInstance;
    private bool hasRevealedPresent = false;

    void Start()
    {
        // Time.timeScale'i kontrol et ve gerekirse düzelt
        if (Time.timeScale != 1f)
        {
            Debug.LogWarning($"Time.timeScale was {Time.timeScale}. Setting it back to 1f.");
            Time.timeScale = 1f;
        }

        // Sahne baþladýðýnda hediye durumu kontrol edilir.
        if (GameStateManager.Instance != null && GameStateManager.Instance.HasPendingPresent)
        {
            // Hediye varsa, hediye gösterme akýþýný baþlat.
            StartCoroutine(ShowPresentRevealFlow());
        }
        else
        {
            // Hediye yoksa, doðrudan birleþtirme masasýný göster.
            ShowMergeTableOnly();
        }
    }

    private IEnumerator ShowPresentRevealFlow()
    {
        // Panelleri ayarla
        presentRevealPanel.SetActive(true);
        mergeTablePanel.SetActive(false);

        // Hediye kutusunu oluþtur ve konumlandýr
        if (presentBoxPrefab != null && presentRevealPosition != null)
        {
            presentInstance = Instantiate(presentBoxPrefab, presentRevealPosition.position, presentRevealPosition.rotation);
            presentInstance.transform.localScale = Vector3.one * presentRevealScale;
        }

        // Týklama için bekle (OnPresentBoxClicked metodu buton tarafýndan çaðrýlacak)
        hasRevealedPresent = false;
        yield return null; // Bu coroutine þimdilik burada durur.
    }

    // Bu metod, 'ClickToOpenPresentButton' butonunun OnClick event'ine atanmalýdýr.
    public void OnPresentBoxClicked()
    {
        if (hasRevealedPresent) return;
        hasRevealedPresent = true;

        // Animasyonu baþlat
        StartCoroutine(RevealAnimation());
    }

    private IEnumerator RevealAnimation()
    {
        // Hediye kutusunu animasyonla yok et
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

        // Bir an bekle, sonra birleþtirme masasýný göster
        yield return new WaitForSeconds(0.5f);
        ShowMergeTableOnly();
    }

    private void ShowMergeTableOnly()
    {
        // Panelleri ayarla
        presentRevealPanel.SetActive(false);
        mergeTablePanel.SetActive(true);

        // Hediye bayraðýný sýfýrla ki bir sonraki sefere tekrar göstermesin.
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.HasPendingPresent = false;
        }

        // Grid'i yenile
        var gridManager = FindObjectOfType<WeaponPartGridManager>();
        if (gridManager != null)
        {
            gridManager.RefreshGrid();
        }
    }

    public void ReturnToGame()
    {
        int nextLevelIndex = 0;
        if (GameStateManager.Instance != null)
        {
            int lastCompletedIndex = GameStateManager.Instance.LastCompletedLevelIndex;
            Debug.Log($"[WeaponMergeSceneController] GameStateManager'den okunan LastCompletedLevelIndex: {lastCompletedIndex}");

            // Bir sonraki seviyenin buildIndex'ini hesapla.
            nextLevelIndex = (lastCompletedIndex == -1) ? 0 : lastCompletedIndex + 1;
        }
        else
        {
            Debug.LogError("[WeaponMergeSceneController] GameStateManager örneði bulunamadý! Varsayýlan olarak seviye 0 yüklenecek.");
        }

        Debug.Log($"[WeaponMergeSceneController] Yüklenecek sonraki seviye indeksi: {nextLevelIndex}");

        // Build Settings'de bir sonraki seviyenin olup olmadýðýný kontrol et.
        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextLevelIndex);
        }
        else
        {
            // Eðer sonraki seviye yoksa, döngüyü baþa al ve ilk seviyeyi (index 0) yükle.
            Debug.LogWarning($"Sonraki seviye (index: {nextLevelIndex}) bulunamadý. Seviye 0'a dönülüyor.");
            SceneManager.LoadScene(0);
        }
    }
}

