using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PresentBoxPickup : MonoBehaviour
{
    public GameObject pickupEffect;
    public float rotationSpeed = 50f;
    public float collectionPauseDuration = 1f;

    private static bool isBeingCollected = false;
    private Animator parentAnimator;
    private Transform parentTransform; // Parent'ýn transform'unu saklamak için

    private void Start()
    {
        isBeingCollected = false;
        parentAnimator = GetComponentInParent<Animator>();
        // Parent'ýn transform referansýný al
        if (transform.parent != null)
        {
            parentTransform = transform.parent;
        }
        else
        {
            // Eðer bir parent yoksa, bu objenin kendisini döndür
            parentTransform = transform;
        }
    }

    void Update()
    {
        if (!isBeingCollected)
        {
            // Döndürme iþlemini child yerine parent'a uygula
            parentTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isBeingCollected && other.CompareTag("Player"))
        {
            isBeingCollected = true;
            StartCoroutine(CollectPresentCoroutine());
        }
    }

    private IEnumerator CollectPresentCoroutine()
    {
        // 1. Animasyonu tetikle
        if (parentAnimator != null)
        {
            parentAnimator.SetTrigger("Collect");
        }

        // 2. Oyunu duraklat ve animasyonun bitmesini bekle
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(collectionPauseDuration);

        // 3. Gerekli verileri ayarla
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.HasPendingPresent = true;
            // Seviye index'ini de burada ayarla, çünkü seviye bu þekilde tamamlandý.
            GameStateManager.Instance.LastCompletedLevelIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log($"[PresentBoxPickup] Seviye tamamlandý (Kutu). Kaydedilen buildIndex: {GameStateManager.Instance.LastCompletedLevelIndex}");
        }

        var inventory = FindObjectOfType<WeaponPartInventory>();
        if (inventory != null)
        {
            inventory.AddRandomTier1Part();
        }

        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        // 4. Sahne geçiþinden hemen önce Time.timeScale'i düzelt
        Time.timeScale = 1f;

        // 5. weaponMerge sahnesine geç
        SceneManager.LoadScene("weaponMerge");

        // 6. Parent objeyi yok et (sahne geçiþinden sonra çalýþmayabilir ama temizlik için kalabilir)
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

