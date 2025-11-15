using UnityEngine;

/// <summary>
/// Helper component to display 3D weapon part models in UI slots
/// Add this to an empty GameObject child of the slot, then assign it to modelContainer
/// </summary>
public class WeaponPartModelDisplay : MonoBehaviour
{
    [Header("Display Settings")]
    public float rotationSpeed = 30f; // Rotate model slowly for visual appeal
    public Vector3 modelOffset = Vector3.zero;
    public float modelScale = 0.5f;
    
    private GameObject currentModel = null;
    
    void Update()
    {
        // Slowly rotate the model if it exists
        if (currentModel != null)
        {
            currentModel.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
    
    public void SetModel(GameObject prefab)
    {
        // Clear existing
        if (currentModel != null)
        {
            Destroy(currentModel);
        }
        
        if (prefab != null)
        {
            currentModel = Instantiate(prefab, transform);
            currentModel.transform.localPosition = modelOffset;
            currentModel.transform.localRotation = Quaternion.identity;
            currentModel.transform.localScale = Vector3.one * modelScale;
            
            // Disable physics components
            Collider[] colliders = currentModel.GetComponentsInChildren<Collider>();
            foreach (var col in colliders) col.enabled = false;
            Rigidbody[] rbs = currentModel.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs) rb.isKinematic = true;
        }
    }
    
    void OnDestroy()
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
        }
    }
}

