using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    private bool isDead = false;
    private Transform colliderTransform;
    private EnemyHealthBar healthBar;

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Set tag for bullet detection
        if (!gameObject.CompareTag("Enemy"))
        {
            gameObject.tag = "Enemy";
        }

        // Set up trigger collider
        SetupCollider();

        // Get or add health bar component
        healthBar = GetComponent<EnemyHealthBar>();
        if (healthBar == null)
        {
            healthBar = gameObject.AddComponent<EnemyHealthBar>();
        }

        Debug.Log($"Enemy '{gameObject.name}' ready with {maxHealth} health");
    }

    void SetupCollider()
    {
        // Check for existing trigger collider
        Collider[] colliders = GetComponentsInChildren<Collider>();
        bool hasTrigger = false;

        foreach (Collider col in colliders)
        {
            if (col.isTrigger)
            {
                hasTrigger = true;
                col.gameObject.tag = "Enemy";
                colliderTransform = col.transform;
                Debug.Log($"Using existing trigger collider on '{col.gameObject.name}'");
                break;
            }
        }

        // Create trigger if none exists
        if (!hasTrigger)
        {
            GameObject triggerObj = new GameObject("EnemyTrigger");
            triggerObj.transform.SetParent(transform);
            triggerObj.transform.localPosition = Vector3.zero;
            triggerObj.tag = "Enemy";

            SphereCollider trigger = triggerObj.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 1.5f;

            colliderTransform = triggerObj.transform;
            Debug.Log("Created new trigger collider");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Health: {currentHealth}/{maxHealth}");

        // Show health bar on first damage
        if (healthBar != null)
        {
            healthBar.Show();
            healthBar.UpdateHealth();
        }

        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    public Vector3 GetColliderPosition()
    {
        return colliderTransform != null ? colliderTransform.position : transform.position;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log($"Enemy '{gameObject.name}' died!");

        // Hide health bar
        if (healthBar != null)
        {
            healthBar.Hide();
        }

        // Fall and destroy
        StartCoroutine(FallDown());
    }

    System.Collections.IEnumerator FallDown()
    {
        float fallSpeed = 5f;

        while (transform.position.y > -5f)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}