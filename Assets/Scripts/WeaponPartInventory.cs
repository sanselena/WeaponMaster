using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponPartInventory : MonoBehaviour
{
    public static WeaponPartInventory Instance { get; private set; }

    private Dictionary<WeaponPartId, int> partCounts = new Dictionary<WeaponPartId, int>();
    private Dictionary<WeaponPartId, WeaponPartDefinition> partDefinitions;
    
    private WeaponPartId equippedHat = WeaponPartId.None;

    public event Action OnInventoryChanged;
    public event Action<WeaponPartId> OnEquippedHatChanged;
    public event Action OnEquippedChanged; 


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[WeaponPartInventory] Instance oluþturuldu ve DontDestroyOnLoad olarak iþaretlendi.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // partDefinitions'ýn yalnýzca bir kez, ilk oluþturmada yüklenmesini garantilemek için
        // kontrol ekliyoruz.
        if (partDefinitions == null)
        {
            InitializeDefinitions();
        }
    }

    private void InitializeDefinitions()
    {
        partDefinitions = new Dictionary<WeaponPartId, WeaponPartDefinition>();
        var defs = Resources.LoadAll<WeaponPartDefinition>("WeaponParts");
        Debug.Log($"[WeaponPartInventory] 'Resources/WeaponParts' klasöründen {defs.Length} adet WeaponPartDefinition yüklendi.");
        foreach (var def in defs)
        {
            if (def != null && !partDefinitions.ContainsKey(def.partId))
            {
                partDefinitions[def.partId] = def;
            }
        }
    }

    // YENÝ METOD: Rastgele bir Tier 1 parça ekler
    public void AddRandomTier1Part()
    {
        // Bütün Tier 1 parçalarýný bul
        var tier1Parts = partDefinitions.Values.Where(p => p.tier == 1).ToList();

        if (tier1Parts.Count > 0)
        {
            // Ýçlerinden rastgele birini seç
            WeaponPartDefinition randomPart = tier1Parts[UnityEngine.Random.Range(0, tier1Parts.Count)];
            AddPart(randomPart.partId, 1);
            Debug.Log($"Rastgele eklendi: {randomPart.displayName}");
        }
        else
        {
            Debug.LogWarning("Envantere eklenecek Tier 1 parça tanýmý bulunamadý!");
        }
    }

    public void TryEquipHat(WeaponPartId newHatId)
    {
        Debug.Log($"[WeaponPartInventory] TryEquipHat çaðrýldý. newHatId: {newHatId}");
        WeaponPartDefinition newHatDef = GetPartDefinition(newHatId);
        if (newHatDef == null)
        {
            Debug.LogError($"[WeaponPartInventory] {newHatId} için taným bulunamadý! Equip iþlemi baþarýsýz.");
            return;
        }

        WeaponPartDefinition currentHatDef = GetPartDefinition(equippedHat);

        if (currentHatDef == null || newHatDef.tier >= currentHatDef.tier)
        {
            equippedHat = newHatId;
            
            Debug.Log($"[WeaponPartInventory] Þapka kuþanýldý! Yeni þapka: {newHatDef.displayName} (Tier: {newHatDef.tier})");

            OnEquippedHatChanged?.Invoke(equippedHat);
            OnEquippedChanged?.Invoke();
            OnInventoryChanged?.Invoke();
        }
        else
        {
            Debug.Log($"Failed to equip {newHatDef.displayName} (Tier: {newHatDef.tier}). Current hat {currentHatDef.displayName} (Tier: {currentHatDef.tier}) is higher tier.");
        }
    }
    
    public WeaponPartId GetEquippedHat()
    {
        Debug.Log($"[WeaponPartInventory] GetEquippedHat çaðrýldý. Dönen deðer: {equippedHat}");
        return equippedHat;
    }

    public float GetEquippedBonus()
    {
        if (equippedHat != WeaponPartId.None)
        {
            var def = GetPartDefinition(equippedHat);
            return def != null ? def.statBonus : 0f;
        }
        return 0f;
    }

    public WeaponPartDefinition GetPartDefinition(WeaponPartId partId)
    {
        if (partDefinitions == null)
        {
            Debug.LogError("[WeaponPartInventory] GetPartDefinition çaðrýldý ancak 'partDefinitions' henüz baþlatýlmamýþ (null)!");
            return null;
        }
        partDefinitions.TryGetValue(partId, out var def);
        Debug.Log($"[WeaponPartInventory] GetPartDefinition({partId}) çaðrýldý. Taným bulundu mu?: {(def != null)}");
        return def;
    }

    public void AddPart(WeaponPartId partId, int amount = 1)
    {
        if (!partCounts.ContainsKey(partId))
        {
            partCounts[partId] = 0;
        }
        partCounts[partId] += amount;
        OnInventoryChanged?.Invoke();
    }

    public void RemovePart(WeaponPartId partId, int amount = 1)
    {
        if (partCounts.ContainsKey(partId))
        {
            partCounts[partId] -= amount;
            if (partCounts[partId] <= 0)
            {
                partCounts.Remove(partId);
            }
            OnInventoryChanged?.Invoke();
        }
    }

    public Dictionary<WeaponPartId, int> GetAllOwnedParts()
    {
        return partCounts;
    }
}

