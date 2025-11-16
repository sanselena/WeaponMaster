using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponPartInventory : MonoBehaviour
{
    public static WeaponPartInventory Instance { get; private set; }

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

    public float GetEquippedPartBonus()
    {
        if (equippedHat != WeaponPartId.None && partDefMap.TryGetValue(equippedHat, out var definition))
        {
            return definition.statBonus;
        }
        return 0f;
    }

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
        if (hatId == equippedHat) return false;

        if (hatId == WeaponPartId.None)
        {
            equippedHat = WeaponPartId.None;
            OnEquippedHatChanged?.Invoke(equippedHat);
            return true;
        }

        if (!ownedParts.ContainsKey(hatId)) return false;

        // Tier düþüklüðünü engelle
        if (equippedHat != WeaponPartId.None)
        {
            var currentDef = GetPartDefinition(equippedHat);
            var newDef = GetPartDefinition(hatId);
            if (currentDef != null && newDef != null && newDef.tier < currentDef.tier)
            {
                return false;
            }
        }

        equippedHat = hatId;
        OnEquippedHatChanged?.Invoke(equippedHat);
        return true;
    }

    public WeaponPartId GetEquippedHat() => equippedHat;

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

    // YENÝ: Envanteri komple temizle
    public void ClearAll()
    {
        ownedParts.Clear();
        TryEquipHat(WeaponPartId.None);
        OnInventoryChanged?.Invoke();
    }
}

