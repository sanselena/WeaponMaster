using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Health Bar")]
    public GameObject healthBarPrefab; // Optional prefab
    public Vector3 healthBarOffset = new Vector3(0, 2.5f, 0); // Above enemy's head
    
    private GameObject healthBarObject;
    private Slider healthSlider;
    private bool isDead = false;
    
    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        
        // Create health bar above enemy
        CreateHealthBar();
        
        // Set up collider for bullet detection (won't interfere with existing components)
        SetupCollider();
        
        // Set tag for bullet detection
        gameObject.tag = "Enemy";
        
        Debug.Log($"Enemy '{gameObject.name}' initialized with {maxHealth} health");
    }
    
    void CreateHealthBar()
    {
        if (healthBarPrefab != null)
        {
            // Use prefab if assigned
            healthBarObject = Instantiate(healthBarPrefab);
        }
        else
        {
            // Create simple health bar
            healthBarObject = new GameObject("HealthBar");
            
            // Add Canvas for UI
            Canvas canvas = healthBarObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            
            // Add Image for background
            GameObject background = new GameObject("Background");
            background.transform.SetParent(healthBarObject.transform);
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = Color.red;
            
            // Add Slider for health
            GameObject sliderObj = new GameObject("HealthSlider");
            sliderObj.transform.SetParent(healthBarObject.transform);
            healthSlider = sliderObj.AddComponent<Slider>();
            
            // Set up slider
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
            healthSlider.value = 1f;
            
            // Add fill image
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(sliderObj.transform);
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = Color.green;
            healthSlider.fillRect = fill.GetComponent<RectTransform>();
        }
        
        // Position health bar above enemy
        healthBarObject.transform.SetParent(transform);
        healthBarObject.transform.localPosition = healthBarOffset;
        healthBarObject.transform.localScale = Vector3.one * 0.01f; // Make it small for world space
    }
    
    void SetupCollider()
    {
        // Add a simple trigger collider for bullet detection
        // This won't interfere with existing mesh colliders
        GameObject triggerObj = new GameObject("EnemyTrigger");
        triggerObj.transform.SetParent(transform);
        triggerObj.transform.localPosition = Vector3.zero;
        triggerObj.transform.localScale = Vector3.one * 1.2f; // Slightly larger than model
        
        SphereCollider trigger = triggerObj.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        
        Debug.Log($"Enemy trigger collider added to '{gameObject.name}'");
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        Debug.Log($"Enemy '{gameObject.name}' took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        // Update health bar
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
        
        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log($"Enemy '{gameObject.name}' died!");
        
        // Hide health bar
        if (healthBarObject != null)
        {
            healthBarObject.SetActive(false);
        }
        
        // Make enemy fall (similar to obstacles)
        StartCoroutine(FallDown());
    }
    
    System.Collections.IEnumerator FallDown()
    {
        float fallSpeed = 5f;
        
        // Make enemy fall below ground
        while (transform.position.y > -5f)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
            yield return null;
        }
        
        // Destroy the enemy after it falls
        Debug.Log($"Enemy '{gameObject.name}' destroyed after falling");
        Destroy(gameObject);
    }
}
