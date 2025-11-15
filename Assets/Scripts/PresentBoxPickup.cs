using UnityEngine;
using UnityEngine.SceneManagement;

public class PresentBoxPickup : MonoBehaviour
{
    public GameObject pickupEffect; // Toplama efekti için partikül sistemi
    public float rotationSpeed = 50f; // Kutunun kendi etrafýnda dönme hýzý

    // Diðer scriptlerin bu kutunun toplanýp toplanmadýðýný bilmesi için statik bir bayrak.
    public static bool isCollected = false;

    private void Start()
    {
        isCollected = false; // Sahne baþladýðýnda bayraðý sýfýrla
    }

    void Update()
    {
        // Kutuyu kendi etrafýnda döndür
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Sadece "Player" etiketine sahip nesnelerle etkileþime gir
        if (other.CompareTag("Player"))
        {
            CollectPresent();
        }
    }

    private void CollectPresent()
    {
        if (isCollected) return; // Zaten toplandýysa tekrar tetiklenmesin
        isCollected = true;

        // Efekti oluþtur (varsa)
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        // Hediye durumunu GameStateManager üzerinden ayarla
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.HasPendingPresent = true;
        }
        else
        {
            Debug.LogError("GameStateManager örneði sahnede bulunamadý!");
        }


        // Envantere yeni bir Tier 1 silah parçasý ekle
        var inventory = FindObjectOfType<WeaponPartInventory>();
        if (inventory != null)
        {
            // HATA BURADAYDI: inventory.AddPart(1) yerine doðru metot çaðrýlmalý.
            inventory.AddRandomTier1Part();
        }
        else
        {
            Debug.LogError("WeaponPartInventory bulunamadý!");
        }

        // Kutuyu yok et
        Destroy(gameObject);

        // KALDIRILDI: Sahne deðiþtirme sorumluluðu LevelManager'a devredildi.
        // SceneManager.LoadScene("weaponMerge");
    }
}

