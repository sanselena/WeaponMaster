using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PresentBoxPickup : MonoBehaviour
{
    public GameObject pickupEffect;
    public float rotationSpeed = 50f;
    public float collectionPauseDuration = 1f;

    private static bool isBeingCollected = false;
    private Animator parentAnimator;
    private Transform parentTransform;

    private void Start()
    {
        isBeingCollected = false;
        parentAnimator = GetComponentInParent<Animator>();
        parentTransform = transform.parent != null ? transform.parent : transform;
    }

    void Update()
    {
        if (!isBeingCollected)
        {
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
        if (parentAnimator != null)
        {
            parentAnimator.SetTrigger("Collect");
        }

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(collectionPauseDuration);

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.HasPendingPresent = true;
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

        Time.timeScale = 1f;

        if (EvolutionState.Instance != null)
        {
            EvolutionState.Instance.UpdateUnlockProgressOnLevelComplete();
        }
        else
        {
            Debug.LogError("[PresentBoxPickup] EvolutionState.Instance is null! Evolution kilidi kontrol edilemedi.");
        }

        SceneManager.LoadScene("weaponMerge");

        if (transform.parent != null) Destroy(transform.parent.gameObject);
        else Destroy(gameObject);
    }
}