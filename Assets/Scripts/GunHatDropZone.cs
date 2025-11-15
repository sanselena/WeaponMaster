using UnityEngine;
using UnityEngine.EventSystems;

public class GunHatDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        WeaponPartSlot sourceSlot = draggedObject.GetComponent<WeaponPartSlot>();
        if (sourceSlot != null)
        {
            // Envantere bu parçayý kuþanmayý denemesini söyle
            WeaponPartInventory.Instance.TryEquipHat(sourceSlot.GetPartId());
        }
    }
}

