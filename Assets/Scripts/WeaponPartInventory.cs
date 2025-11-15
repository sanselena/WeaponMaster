using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponPartInventory : MonoBehaviour
{
    public static WeaponPartInventory Instance { get; private set; }

    // Mevcut event'ler ve deðiþkenler korunuyor
    public event Action<WeaponPartId> OnEquippedHatChanged;
    public event Action OnInventoryChanged;

    private Dictionary<WeaponPartId, int> ownedParts = new Dictionary<WeaponPartId, int>();
    private WeaponPartId equippedHat = WeaponPartId.None;
    private Dictionary<WeaponPartId, WeaponPartDefinition> partDefMap;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPartDefinitions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadPartDefinitions()
    {
        partDefMap = new Dictionary<WeaponPartId, WeaponPartDefinition>();
        var defs = Resources.LoadAll<WeaponPartDefinition>("WeaponParts");
        foreach (var def in defs)
        {
            if (def != null && !partDefMap.ContainsKey(def.partId))
            {
                partDefMap.Add(def.partId, def);
            }
        }
    }

    // YENÝ METOT: Kuþanýlmýþ parçanýn stat bonusunu döndürür.
    public float GetEquippedPartBonus()
    {
        if (equippedHat != WeaponPartId.None && partDefMap.TryGetValue(equippedHat, out var definition))
        {
            return definition.statBonus;
        }
        return 0f;
    }

    // --- Mevcut Metotlar Deðiþtirilmeden Korunuyor ---

    public void AddPart(WeaponPartId partId, int quantity = 1)
    {
        if (partId == WeaponPartId.None) return;
        if (ownedParts.ContainsKey(partId))
            ownedParts[partId] += quantity;
        else
            ownedParts[partId] = quantity;
        OnInventoryChanged?.Invoke();
    }

    public void RemovePart(WeaponPartId partId, int quantity = 1)
    {
        if (ownedParts.ContainsKey(partId))
        {
            ownedParts[partId] -= quantity;
            if (ownedParts[partId] <= 0)
            {
                ownedParts.Remove(partId);
                if (equippedHat == partId)
                {
                    TryEquipHat(WeaponPartId.None);
                }
            }
            OnInventoryChanged?.Invoke();
        }
    }

    public bool TryEquipHat(WeaponPartId hatId)
    {
        if (hatId != equippedHat && (hatId == WeaponPartId.None || ownedParts.ContainsKey(hatId)))
        {
            equippedHat = hatId;
            OnEquippedHatChanged?.Invoke(equippedHat);
            return true;
        }
        return false;
    }

    public WeaponPartId GetEquippedHat()
    {
        return equippedHat;
    }

    public WeaponPartDefinition GetPartDefinition(WeaponPartId partId)
    {
        partDefMap.TryGetValue(partId, out var def);
        return def;
    }

    public Dictionary<WeaponPartId, int> GetAllOwnedParts()
    {
        return new Dictionary<WeaponPartId, int>(ownedParts);
    }

    public void AddRandomTier1Part()
    {
        var tier1Parts = partDefMap.Values.Where(p => p.tier == 1).ToList();
        if (tier1Parts.Any())
        {
            var randomPart = tier1Parts[UnityEngine.Random.Range(0, tier1Parts.Count)];
            AddPart(randomPart.partId);
        }
    }
}

