using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float fallSpeed = 5f;
    private bool isShot = false;
    
    void Start()
    {
        // Make sure obstacle has a collider for shooting
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
        }
        
        // Make sure it's a trigger for bullet detection
        col.isTrigger = true;
        
        // Set tag for identification
        gameObject.tag = "Obstacle";
        
        //Debug.Log($"Obstacle '{gameObject.name}' set up with trigger collider");
    }
    
    public void GetShot()
    {
        if (!isShot)
        {
            isShot = true;
            Debug.Log($"Obstacle '{gameObject.name}' shot! Falling...");
            StartCoroutine(FallDown());
        }
    }
    
    System.Collections.IEnumerator FallDown()
    {
        // Make obstacle fall below ground
        while (transform.position.y > -5f)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
            yield return null;
        }
        
        // Destroy the obstacle after it falls
        Debug.Log($"Obstacle '{gameObject.name}' destroyed after falling");
        Destroy(gameObject);
    }
}
