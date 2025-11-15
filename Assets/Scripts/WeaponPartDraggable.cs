using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponPartDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private static GameObject dragIconInstance; // Sürüklenen görsel ikon (statik)
    private static int originalRenderQueue = -1; // Orijinal render sýrasýný saklamak için

    private WeaponPartSlot parentSlot;
    private Canvas rootCanvas;

    void Awake()
    {
        parentSlot = GetComponent<WeaponPartSlot>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (parentSlot == null || parentSlot.GetPartId() == WeaponPartId.None)
        {
            eventData.pointerDrag = null;
            return;
        }

        var definition = parentSlot.GetPartDefinition();
        if (definition != null && definition.worldPrefab != null)
        {
            // --- GÜNCELLEME BAÞLANGICI ---

            // 1. Geçici görseli oluþtur
            dragIconInstance = Instantiate(definition.worldPrefab, rootCanvas.transform);
            
            // 2. Katmanýný "UI" olarak ayarla ki UI kamerasý onu çizebilsin
            //    Bu, modelin ve tüm alt objelerinin katmanýný deðiþtirir.
            SetLayerRecursively(dragIconInstance, LayerMask.NameToLayer("UI"));

            // 3. Sürüklenen ikonun fare týklamalarýný engellemesini önle
            var canvasGroup = dragIconInstance.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;

            // 4. Eðer bir RectTransform'u yoksa, ekle. Bu, UI içinde konumlanmasý için gereklidir.
            if (dragIconInstance.GetComponent<RectTransform>() == null)
            {
                dragIconInstance.AddComponent<RectTransform>();
            }
            
            // YENÝ: Sürüklenen objenin her zaman üstte görünmesini saðla
            SetRenderQueue(dragIconInstance, 3002); // UI elementlerinden daha yüksek bir deðer

            // --- GÜNCELLEME SONU ---

            // Orijinal slot'taki modeli gizle
            parentSlot.SetModelVisibility(false);
            
            // Sürüklenen ikonun boyutunu ayarla
            dragIconInstance.transform.localScale = Vector3.one * definition.uiScale;

            UpdateDraggedPosition(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconInstance != null)
        {
            UpdateDraggedPosition(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // YENÝ: Render sýrasýný orijinal haline getir
        if (originalRenderQueue != -1 && dragIconInstance != null)
        {
            SetRenderQueue(dragIconInstance, originalRenderQueue);
            originalRenderQueue = -1;
        }

        if (dragIconInstance != null)
        {
            Destroy(dragIconInstance);
        }

        GameObject droppedOn = eventData.pointerEnter;
        // Býrakýlan yer geçerli bir slot deðilse veya boþ bir alansa, modeli geri göster.
        if (droppedOn == null || droppedOn.GetComponentInParent<WeaponPartSlot>() == null)
        {
             parentSlot.SetModelVisibility(true);
        }
        // Eðer geçerli bir slota býrakýldýysa, OnDrop metodu zaten envanteri güncelleyecek
        // ve RefreshGrid her þeyi yeniden çizecektir. Bu yüzden modeli geri göstermeye gerek yok.
    }

    private void UpdateDraggedPosition(PointerEventData eventData)
    {
        if (dragIconInstance.transform is RectTransform rectTransform)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rootCanvas.transform as RectTransform, 
                eventData.position, 
                eventData.pressEventCamera, 
                out Vector2 localPosition);
            rectTransform.localPosition = localPosition;
        }
    }

    // Bir objenin ve tüm alt objelerinin katmanýný deðiþtiren yardýmcý fonksiyon
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    // YENÝ: Objenin ve alt objelerinin render sýrasýný deðiþtiren yardýmcý fonksiyon
    private void SetRenderQueue(GameObject obj, int queue)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if (renderer.material != null)
            {
                if (originalRenderQueue == -1) // Sadece ilk seferde orijinali kaydet
                {
                    originalRenderQueue = renderer.material.renderQueue;
                }
                renderer.material.renderQueue = queue;
            }
        }
    }
}

