using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponPartGridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public Transform gridParent;
    public GameObject slotPrefab;
    public int gridWidth = 4;
    public int gridHeight = 4;

    [Header("Part Definitions")]
    public WeaponPartDefinition[] partDefinitions;

    private List<WeaponPartSlot> slots = new List<WeaponPartSlot>();
    private Dictionary<WeaponPartId, WeaponPartDefinition> partDefMap;

    void Awake()
    {
        partDefMap = new Dictionary<WeaponPartId, WeaponPartDefinition>();
        foreach (var def in partDefinitions)
        {
            if (def != null && !partDefMap.ContainsKey(def.partId))
            {
                partDefMap[def.partId] = def;
            }
        }
    }

    void Start()
    {
        CreateGrid();
        if (WeaponPartInventory.Instance != null)
        {
            WeaponPartInventory.Instance.OnInventoryChanged += RefreshGrid;
            Debug.Log("GridManager subscribed to OnInventoryChanged.");
        }
        RefreshGrid();
    }

    private void OnDestroy()
    {
        if (WeaponPartInventory.Instance != null)
        {
            WeaponPartInventory.Instance.OnInventoryChanged -= RefreshGrid;
        }
    }

    void CreateGrid()
    {
        foreach (Transform child in gridParent) Destroy(child.gameObject);
        slots.Clear();

        for (int i = 0; i < gridWidth * gridHeight; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, gridParent);
            WeaponPartSlot slot = slotGO.GetComponent<WeaponPartSlot>();
            
            // DÜZELTME: Initialize metoduna sadece manager'ý gönder
            slot.Initialize(this); 
            
            slots.Add(slot);
        }
        Debug.Log($"Created {slots.Count} slots in the grid.");
    }

    public void RefreshGrid()
    {
        Debug.Log("RefreshGrid: Refreshing all slots.");
        if (WeaponPartInventory.Instance == null)
        {
            Debug.LogWarning("RefreshGrid: WeaponPartInventory.Instance is null.");
            return;
        }

        var ownedParts = WeaponPartInventory.Instance.GetAllOwnedParts();
        
        int slotIndex = 0;
        foreach (var kvp in ownedParts.OrderBy(p => p.Key))
        {
            if (kvp.Key == WeaponPartId.None) continue;
            for (int i = 0; i < kvp.Value && slotIndex < slots.Count; i++)
            {
                slots[slotIndex].SetPart(kvp.Key);
                slotIndex++;
            }
        }

        for (int i = slotIndex; i < slots.Count; i++)
        {
            slots[i].SetPart(WeaponPartId.None);
        }
        Debug.Log("RefreshGrid: Grid refresh complete.");
    }

    public void HandleDropMerge(WeaponPartSlot sourceSlot, WeaponPartSlot destinationSlot)
    {
        WeaponPartId part1 = sourceSlot.GetPartId();
        WeaponPartId part2 = destinationSlot.GetPartId();

        // HATA DÜZELTMESÝ: CanMerge metodunu 3 parametre ile çaðýr.
        if (WeaponPartMerger.CanMerge(part1, part2, out WeaponPartId resultPart))
        {
            var inventory = WeaponPartInventory.Instance;
            if (inventory == null) return;

            Debug.Log($"HandleDropMerge: Merging {part1} and {part2} into {resultPart}.");
            
            inventory.RemovePart(part1);
            inventory.RemovePart(part2);
            inventory.AddPart(resultPart);
        }
        else
        {
            Debug.Log($"HandleDropMerge: Cannot merge {part1} and {part2}.");
            sourceSlot.SetModelVisibility(true);
        }
    }

    public WeaponPartDefinition GetPartDefinition(WeaponPartId partId)
    {
        partDefMap.TryGetValue(partId, out WeaponPartDefinition def);
        return def;
    }
}

