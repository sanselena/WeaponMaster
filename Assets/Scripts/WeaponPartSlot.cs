using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class WeaponPartSlot : MonoBehaviour, IDropHandler
{
    [Header("UI References")]
    public TextMeshProUGUI partNameText;
    public TextMeshProUGUI countText;
    public Transform modelContainer;

    private WeaponPartId currentPart = WeaponPartId.None;
    private WeaponPartGridManager gridManager;
    private GameObject currentModelInstance = null;

    public void Initialize(WeaponPartGridManager manager)
    {
        gridManager = manager;
    }

    public void SetPart(WeaponPartId partId)
    {
        currentPart = partId;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
        }
        
        if (currentPart == WeaponPartId.None)
        {
            if (partNameText != null) partNameText.gameObject.SetActive(false);
            if (countText != null) countText.gameObject.SetActive(false);
            if (modelContainer != null) modelContainer.gameObject.SetActive(false);
        }
        else
        {
            WeaponPartDefinition def = gridManager?.GetPartDefinition(currentPart);
            if (def == null) return;

            if (partNameText != null)
            {
                partNameText.gameObject.SetActive(true);
                partNameText.text = def.displayName;
            }
            if (countText != null)
            {
                countText.gameObject.SetActive(false);
            }
            
            if (modelContainer != null && def.worldPrefab != null)
            {
                modelContainer.gameObject.SetActive(true);
                currentModelInstance = Instantiate(def.worldPrefab, modelContainer);
                
                // ScriptableObject'ten gelen özel duruþ ayarlarýný uygula
                currentModelInstance.transform.localPosition = Vector3.zero;
                currentModelInstance.transform.localRotation = Quaternion.Euler(def.uiRotation);
                currentModelInstance.transform.localScale = Vector3.one * def.uiScale;
                
                DisableModelComponents(currentModelInstance);
            }
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            WeaponPartSlot sourceSlot = eventData.pointerDrag.GetComponent<WeaponPartSlot>();
            if (sourceSlot != null && sourceSlot != this && sourceSlot.GetPartId() != WeaponPartId.None)
            {
                gridManager.HandleDropMerge(sourceSlot, this);
            }
        }
    }

    public void SetModelVisibility(bool isVisible)
    {
        if (modelContainer != null)
        {
            modelContainer.gameObject.SetActive(isVisible);
        }
    }

    private void DisableModelComponents(GameObject model)
    {
        if (model.GetComponent<WeaponPartDraggable>() != null) Destroy(model.GetComponent<WeaponPartDraggable>());
        if (model.GetComponent<CanvasGroup>() != null) Destroy(model.GetComponent<CanvasGroup>());
        foreach (var collider in model.GetComponentsInChildren<Collider>()) collider.enabled = false;
        foreach (var rb in model.GetComponentsInChildren<Rigidbody>()) rb.isKinematic = true;
    }

    public WeaponPartDefinition GetPartDefinition() => gridManager?.GetPartDefinition(currentPart);
    public WeaponPartId GetPartId() => currentPart;
}

