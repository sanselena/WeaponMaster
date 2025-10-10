using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 startPosition;
    private float bulletRange; // Set by GunShooting script
    private float bulletPower; // Set by GunShooting script
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    // Method to set bullet properties (called by GunShooting)
    public void SetBulletProperties(float power, float range)
    {
        bulletPower = power;
        bulletRange = range;
    }
    
    void Update()
    {
        // Destroy bullet if it travels beyond range
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= bulletRange)
        {
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log($"Bullet (Power: {bulletPower}) hit obstacle!");
            
            // Tell obstacle it was shot
            Obstacle obstacle = other.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                obstacle.GetShot();
            }
            
            // Destroy bullet
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            Debug.Log($"Bullet (Power: {bulletPower}) hit enemy!");
            
            // Find enemy script (might be on parent or child)
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null)
            {
                enemy = other.GetComponentInParent<Enemy>();
            }
            if (enemy == null)
            {
                enemy = other.GetComponentInChildren<Enemy>();
            }
            
            if (enemy != null)
            {
                enemy.TakeDamage(bulletPower);
            }
            
            // Destroy bullet
            Destroy(gameObject);
        }
    }
}
