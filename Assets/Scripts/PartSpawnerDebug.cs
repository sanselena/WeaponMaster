using UnityEngine;

public class PartSpawnerDebug : MonoBehaviour
{
    // BU METOD, BUTONLARIN ONCLICK EVENT'ÝNDEN ÇAÐRILIR.
    // Inspector'dan her butona ilgili WeaponPartDefinition asset'i atanýr.
    public void AddPart(WeaponPartDefinition partDefinition)
    {
        if (partDefinition == null)
        {
            Debug.LogError("AddPart metoduna null bir partDefinition gönderildi!");
            return;
        }

        if (WeaponPartInventory.Instance != null)
        {
            // Envantere part'ýn ID'sini kullanarak ekle
            WeaponPartInventory.Instance.AddPart(partDefinition.partId, 1);
            Debug.Log($"Spawned: {partDefinition.displayName}");
        }
        else
        {
            Debug.LogError("WeaponPartInventory instance not found!");
        }
    }
}