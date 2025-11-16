using UnityEngine;

public class GunAttachmentManager : MonoBehaviour
{
    public Transform hatAttachmentPoint;
    private GameObject currentHatInstance;

    [Header("Preview Safety")]
    public bool stripPhysicsFromHat = true;
    public bool setIgnoreRaycastLayerForHat = true;

    void OnEnable()
    {
        Debug.Log("[GunAttachmentManager] OnEnable çalýþtý. Nesne aktif.", gameObject);
        if (hatAttachmentPoint == null)
        {
            var found = FindChildByName(transform, "HatAnchor");
            if (found != null) hatAttachmentPoint = found;
        }

        if (hatAttachmentPoint == null)
        {
            Debug.LogError("[GunAttachmentManager] 'hatAttachmentPoint' atanmamýþ! Lütfen Gun prefab'ýný kontrol edip referansý atayýn.", gameObject);
            return;
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
            currentHatInstance = null;
        }

        if (hatId == WeaponPartId.None) return;
        if (hatAttachmentPoint == null) return;

        var definition = WeaponPartInventory.Instance.GetPartDefinition(hatId);
        if (definition != null && definition.worldPrefab != null)
        {
            currentHatInstance = Instantiate(definition.worldPrefab, hatAttachmentPoint);
            currentHatInstance.transform.localPosition = new Vector3(0, definition.attachmentYOffset, 0);
            currentHatInstance.transform.localRotation = Quaternion.Euler(definition.attachmentRotation);
            currentHatInstance.transform.localScale = Vector3.one * definition.attachmentScale;

            if (stripPhysicsFromHat)
            {
                foreach (var c in currentHatInstance.GetComponentsInChildren<Collider>(true)) Destroy(c);
                foreach (var rb in currentHatInstance.GetComponentsInChildren<Rigidbody>(true)) Destroy(rb);
                foreach (var au in currentHatInstance.GetComponentsInChildren<AudioSource>(true)) Destroy(au);
            }
            if (setIgnoreRaycastLayerForHat)
            {
                foreach (var t in currentHatInstance.GetComponentsInChildren<Transform>(true))
                    t.gameObject.layer = 2; // Ignore Raycast
            }

            Debug.Log($"[GunAttachmentManager] '{definition.displayName}' baþarýyla oluþturuldu ve ayarlandý.", currentHatInstance);
        }
        else
        {
            Debug.LogError($"[GunAttachmentManager] Þapka görseli oluþturulamadý! Taným: {(definition != null)}, Prefab: {(definition?.worldPrefab != null)}");
        }
    }

    private Transform FindChildByName(Transform root, string childName)
    {
        if (root.name == childName) return root;
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == childName) return t;
        }
        return null;
    }
}

