using UnityEngine;
using UnityEngine.InputSystem;

public class GunShooting : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float bulletSpeed = 20f;
    public float fireRate = 0.3f;
    public float bulletPower = 1f;
    public float bulletRange = 50f;
    
    [Header("Auto Shooting")]
    public bool autoShoot = true;

    [Header("Visuals")]
    public Material bulletMat;

    [Header("Audio")]
    public AudioClip bulletSFX;
    public AudioSource bulletSFXSource;
    public TeleportingButton bulletMuted;

    private Transform firePoint;
    private float nextFireTime = 0f;
    
    private float baseFireRate;
    private float baseBulletPower;
    private float baseBulletRange;
    
    void Awake()
    {
        baseFireRate = fireRate;
        baseBulletPower = bulletPower;
        baseBulletRange = bulletRange;
    }

    void Start()
    {
        // --- GunStats'tan taþýnan mantýk ---
        if (WeaponPartInventory.Instance != null)
        {
            WeaponPartInventory.Instance.OnEquippedHatChanged += UpdateStatsFromInventory;
            UpdateStatsFromInventory(WeaponPartInventory.Instance.GetEquippedHat());
        }
        // ------------------------------------

        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(0, 0, 1);
            firePoint = firePointObj.transform;
        }

        bulletSFXSource = gameObject.AddComponent<AudioSource>();
        bulletSFXSource.playOnAwake = false;
        
        Debug.Log("Gun shooting system ready! Auto-shooting enabled.");
    }

    void OnDestroy()
    {
        // --- GunStats'tan taþýnan mantýk ---
        if (WeaponPartInventory.Instance != null)
        {
            WeaponPartInventory.Instance.OnEquippedHatChanged -= UpdateStatsFromInventory;
        }
        // ------------------------------------
    }

    // --- GunStats'tan taþýnan metot ---
    private void UpdateStatsFromInventory(WeaponPartId equippedPartId)
    {
        float bonus = 0;
        if (WeaponPartInventory.Instance != null)
        {
            bonus = WeaponPartInventory.Instance.GetEquippedPartBonus();
        }
        ApplyBonus(bonus);
    }
    // ------------------------------------

    public void ApplyBonus(float bonus)
    {
        float bonusMultiplier = 1.0f + (bonus / 100.0f);
        fireRate = baseFireRate / bonusMultiplier;
        bulletPower = baseBulletPower * bonusMultiplier;
        bulletRange = baseBulletRange * bonusMultiplier;
        Debug.Log($"Bonus Applied: {bonus}%. New Stats -> FireRate: {fireRate}, Power: {bulletPower}, Range: {bulletRange}");
    }
    
    void Update()
    {
        if (autoShoot && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
        bulletSFXSource.playOnAwake = false;
    }
    
    void Shoot()
    {
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);   
        bullet.name = "Bullet";
        bullet.transform.position = firePoint.position;
        bullet.transform.localScale = Vector3.one * 0.2f;

        Renderer bulletRenderer = bullet.GetComponent<Renderer>();
        bulletRenderer.material = bulletMat; 

        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = firePoint.forward * bulletSpeed;

        Collider col = bullet.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.SetBulletProperties(bulletPower, bulletRange);

        if (bulletSFX != null)
            bulletSFXSource.PlayOneShot(bulletSFX);
    }
}
