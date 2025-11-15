using UnityEngine;

public class GunAttachmentManager : MonoBehaviour
{
    public Transform hatAttachmentPoint;
    private GameObject currentHatInstance;

    void OnEnable()
    {
        Debug.Log("[GunAttachmentManager] OnEnable çalýþtý. Nesne aktif.", gameObject);
        // Baðlantý noktasýnýn var olup olmadýðýný kontrol edelim.
        if (hatAttachmentPoint == null)
        {
            Debug.LogError("[GunAttachmentManager] 'hatAttachmentPoint' atanmamýþ! Lütfen Gun prefab'ýný kontrol edip referansý atayýn.", gameObject);
            return; // Baðlantý noktasý yoksa devam etme.
        }

        if (WeaponPartInventory.Instance != null)
        {
            Debug.Log("[GunAttachmentManager] Olaylara abone olunuyor ve mevcut þapka güncelleniyor.");
            WeaponPartInventory.Instance.OnEquippedHatChanged += UpdateHatVisual;
            UpdateHatVisual(WeaponPartInventory.Instance.GetEquippedHat());
        }
        else
        {
            Debug.LogError("[GunAttachmentManager] WeaponPartInventory.Instance bulunamadý!");
        }
    }

    void OnDisable()
    {
        Debug.Log("[GunAttachmentManager] OnDisable çalýþtý. Nesne devre dýþý.", gameObject);
        if (WeaponPartInventory.Instance != null)
        {
            Debug.Log("[GunAttachmentManager] Olay aboneliði kaldýrýlýyor.");
            WeaponPartInventory.Instance.OnEquippedHatChanged -= UpdateHatVisual;
        }
    }

    public void UpdateHatVisual(WeaponPartId hatId)
    {
        Debug.Log($"[GunAttachmentManager] UpdateHatVisual çaðrýldý. hatId: {hatId}", gameObject);
        if (currentHatInstance != null)
        {
            Destroy(currentHatInstance);
        }

        if (hatId == WeaponPartId.None)
        {
            return;
        }

        if (hatAttachmentPoint == null) return;

        var definition = WeaponPartInventory.Instance.GetPartDefinition(hatId);
        if (definition != null && definition.worldPrefab != null)
        {
            currentHatInstance = Instantiate(definition.worldPrefab, hatAttachmentPoint);
            
            currentHatInstance.transform.localPosition = new Vector3(0, definition.attachmentYOffset, 0);
            currentHatInstance.transform.localRotation = Quaternion.Euler(definition.attachmentRotation);
            currentHatInstance.transform.localScale = Vector3.one * definition.attachmentScale;
            Debug.Log($"[GunAttachmentManager] '{definition.displayName}' baþarýyla oluþturuldu ve ayarlandý.", currentHatInstance);
        }
        else
        {
            Debug.LogError($"[GunAttachmentManager] Þapka görseli oluþturulamadý! Taným: {(definition != null)}, Prefab: {(definition?.worldPrefab != null)}");
        }
    }
}

