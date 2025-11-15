using UnityEngine;

/// <summary>
/// Helper script to ensure gun has proper collider and rigidbody setup for trigger detection
/// Add this to your Gun GameObject if trigger detection isn't working
/// </summary>
public class GunColliderSetup : MonoBehaviour
{
    void Start()
    {
        // Ensure we have a Rigidbody (needed for trigger detection)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("Gun: No Rigidbody found! Adding kinematic Rigidbody for trigger detection.");
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Won't affect movement, but needed for triggers
            rb.useGravity = false;
        }
        
        // Ensure we have a collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning("Gun: No Collider found! This might cause trigger issues.");
        }
        else
        {
            Debug.Log($"Gun: Collider found - Type: {col.GetType().Name}, IsTrigger: {col.isTrigger}");
        }
        
        // Ensure tag is set
        if (!gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("Gun: Tag is not 'Player'! Setting it now.");
            gameObject.tag = "Player";
        }
        
        Debug.Log($"Gun setup complete - Tag: {gameObject.tag}, Has Rigidbody: {rb != null}, Has Collider: {col != null}");
    }
}

