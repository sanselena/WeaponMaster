using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class GunModelSwitcher : MonoBehaviour
{
    [Header("Model Root (instantiate target)")]
    public Transform modelRoot;

    [Header("Prefabs (VISUAL-ONLY!)")]
    public GameObject baseModelPrefab;
    public GameObject evolvedModelPrefab;

    [Header("Anchor Names")]
    public string muzzleTransformName = "Muzzle";
    public string hatAnchorTransformName = "HatAnchor";

    [Header("Safety")]
    public bool stripPhysicsFromSpawnedModel = true;

    private GameObject currentModel;
    private GunShooting shooting;
    private GunAttachmentManager attachmentManager;
    private bool spawnedOnce;

    void Awake()
    {
        shooting = GetComponent<GunShooting>();
        attachmentManager = GetComponent<GunAttachmentManager>();

        HideRootRendererIfAny();

        if (modelRoot == null)
        {
            var root = new GameObject("ModelRoot");
            root.transform.SetParent(transform, false);
            modelRoot = root.transform;
        }

        if (!spawnedOnce)
        {
            SpawnCorrectModel();
            spawnedOnce = true;
        }
    }

    void OnEnable()
    {
        if (EvolutionState.Instance != null)
        {
            EvolutionState.Instance.OnVariantChanged += OnVariantChanged;
        }
    }

    void OnDisable()
    {
        if (EvolutionState.Instance != null)
        {
            EvolutionState.Instance.OnVariantChanged -= OnVariantChanged;
        }
    }

    private void OnVariantChanged(GunVariant v)
    {
        RefreshModel();
    }

    // DIÞARIDAN ÇAÐRILABÝLÝR: Geçerli varyanta göre görsel modeli yeniler
    public void RefreshModel()
    {
        SpawnCorrectModel();
    }

    private void SpawnCorrectModel()
    {
        if (currentModel != null) Destroy(currentModel);

        bool evolved = EvolutionState.Instance != null && EvolutionState.Instance.CurrentGunVariant == GunVariant.Evolved;
        GameObject prefab = evolved ? evolvedModelPrefab : baseModelPrefab;

        if (!ValidateModelPrefab(prefab))
        {
            Debug.LogError("[GunModelSwitcher] HATALI model prefab atamasý. Prefab SADECE görsel olmalý ve Gun* scriptleri içermemeli.", this);
            return;
        }

        currentModel = Instantiate(prefab, modelRoot);
        currentModel.name = evolved ? "GunModel_Evolved" : "GunModel_Base";
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;
        currentModel.transform.localScale = Vector3.one;

        if (stripPhysicsFromSpawnedModel)
        {
            StripNonVisualComponents(currentModel);
        }

        AttachAnchors();
    }

    private void AttachAnchors()
    {
        Transform muzzle = FindChildByName(currentModel?.transform, muzzleTransformName);
        Transform hatAnchor = FindChildByName(currentModel?.transform, hatAnchorTransformName);

        if (muzzle == null) muzzle = FindChildByName(transform, muzzleTransformName);
        if (hatAnchor == null) hatAnchor = FindChildByName(transform, hatAnchorTransformName);

        if (shooting != null && muzzle != null)
        {
            shooting.SetFirePoint(muzzle);
        }

        if (attachmentManager != null && hatAnchor != null)
        {
            attachmentManager.hatAttachmentPoint = hatAnchor;
        }
    }

    private void HideRootRendererIfAny()
    {
        var mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;
    }

    private bool ValidateModelPrefab(GameObject prefab)
    {
        if (prefab == null) return false;

        if (prefab.GetComponentInChildren<GunModelSwitcher>(true) != null) return false;
        if (prefab.GetComponentInChildren<GunShooting>(true) != null) return false;
        if (prefab.GetComponentInChildren<GunAttachmentManager>(true) != null) return false;
        if (prefab.GetComponentInChildren<WeaponPartInventory>(true) != null) return false;

        var foundColliders = prefab.GetComponentsInChildren<Collider>(true);
        var foundRB = prefab.GetComponentsInChildren<Rigidbody>(true);
        if ((foundColliders != null && foundColliders.Length > 0) || (foundRB != null && foundRB.Length > 0))
        {
            Debug.LogWarning("[GunModelSwitcher] Uyarý: Görsel prefab'ta Collider/Rigidbody bulundu. Spawn sonrasý temizlenecek.", prefab);
        }
        return true;
    }

    private void StripNonVisualComponents(GameObject root)
    {
        int removed = 0;
        foreach (var c in root.GetComponentsInChildren<Collider>(true)) { Destroy(c); removed++; }
        foreach (var rb in root.GetComponentsInChildren<Rigidbody>(true)) { Destroy(rb); removed++; }
        foreach (var au in root.GetComponentsInChildren<AudioSource>(true)) { Destroy(au); removed++; }
        if (removed > 0)
        {
            Debug.Log($"[GunModelSwitcher] Non-visual bileþenler temizlendi. Kaldýrýlan sayýsý: {removed}", this);
        }
    }

    private Transform FindChildByName(Transform root, string childName)
    {
        if (root == null || string.IsNullOrEmpty(childName)) return null;
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == childName) return t;
        }
        return null;
    }
}