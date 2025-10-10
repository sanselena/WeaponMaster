using UnityEngine;
using UnityEngine.InputSystem;

public class GunShooting : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float bulletSpeed = 20f;
    public float fireRate = 0.3f; // Time between shots (lower = faster shooting)
    public float bulletPower = 1f; // Future use for damage/power
    public float bulletRange = 50f; // How far bullets travel before destroying
    
    [Header("Auto Shooting")]
    public bool autoShoot = true; // Start shooting automatically
    
    private Transform firePoint;
    private float nextFireTime = 0f;
    
    void Start()
    {
        // Create fire point if it doesn't exist
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(0, 0, 1); // In front of gun
            firePoint = firePointObj.transform;
        }
        
        Debug.Log("Gun shooting system ready! Auto-shooting enabled.");
    }
    
    void Update()
    {
        // Automatic shooting only
        if (autoShoot && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    void Shoot()
    {
        // Create simple bullet (small sphere)
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.name = "Bullet";
        bullet.transform.position = firePoint.position;
        bullet.transform.localScale = Vector3.one * 0.2f; // Small bullet
        
        // Add rigidbody with no gravity (no recoil/falling)
        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false; // No gravity = no recoil/falling
        rb.linearVelocity = firePoint.forward * bulletSpeed;
        
        // Add bullet script and set properties
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.SetBulletProperties(bulletPower, bulletRange);
        
        Debug.Log($"Bullet fired! Speed: {bulletSpeed}, Power: {bulletPower}, Range: {bulletRange}");
    }
}
