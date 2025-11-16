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

    [Header("Evolved Variant Overrides")]
    public float evolvedBulletSpeed = 25f;
    public float evolvedFireRate = 0.2f;
    public float evolvedBulletPower = 2f;
    public float evolvedBulletRange = 60f;

    private Transform firePoint;
    private float nextFireTime = 0f;
    
    private float baseFireRate;
    private float baseBulletPower;
    private float baseBulletRange;
    private float baseBulletSpeed;
    
    void Awake()
    {
        ApplyVariantBaseStats();
    }

    void OnEnable()
    {
        if (WeaponPartInventory.Instance != null)
        {
            WeaponPartInventory.Instance.OnEquippedHatChanged += UpdateStatsFromInventory;
        }
        if (EvolutionState.Instance != null)
        {
            EvolutionState.Instance.OnVariantChanged += OnVariantChanged;
        }
    }

    void Start()
    {
        if (WeaponPartInventory.Instance != null)
        {
            UpdateStatsFromInventory(WeaponPartInventory.Instance.GetEquippedHat());
        }

        if (firePoint == null)
        {
            var firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(0, 0, 1);
            firePoint = firePointObj.transform;
        }

        bulletSFXSource = gameObject.AddComponent<AudioSource>();
        bulletSFXSource.playOnAwake = false;
        
        Debug.Log("Gun shooting system ready! Auto-shooting enabled.");
    }

    void OnDisable()
    {
        if (WeaponPartInventory.Instance != null)
        {
            WeaponPartInventory.Instance.OnEquippedHatChanged -= UpdateStatsFromInventory;
        }
        if (EvolutionState.Instance != null)
        {
            EvolutionState.Instance.OnVariantChanged -= OnVariantChanged;
        }
    }

    void OnDestroy()
    {
        if (WeaponPartInventory.Instance != null)
        {
            WeaponPartInventory.Instance.OnEquippedHatChanged -= UpdateStatsFromInventory;
        }
        if (EvolutionState.Instance != null)
        {
            EvolutionState.Instance.OnVariantChanged -= OnVariantChanged;
        }
    }

    private void OnVariantChanged(GunVariant v)
    {
        ReapplyVariantBaseStatsAndBonus();
    }

    private void UpdateStatsFromInventory(WeaponPartId equippedPartId)
    {
        float bonus = 0;
        if (WeaponPartInventory.Instance != null)
        {
            bonus = WeaponPartInventory.Instance.GetEquippedPartBonus();
        }
        ApplyBonus(bonus);
    }

    public void ApplyBonus(float bonus)
    {
        float bonusMultiplier = 1.0f + (bonus / 100.0f);

        fireRate     = baseFireRate    / bonusMultiplier;
        bulletPower  = baseBulletPower * bonusMultiplier;
        bulletRange  = baseBulletRange * bonusMultiplier;
        bulletSpeed  = baseBulletSpeed * bonusMultiplier;

        Debug.Log($"Bonus Applied: {bonus}%. New Stats -> FireRate: {fireRate}, Power: {bulletPower}, Range: {bulletRange}, Speed: {bulletSpeed}");
    }

    public void SetFirePoint(Transform t)
    {
        firePoint = t;
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
        var bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.name = "Bullet";
        bullet.transform.position = firePoint.position;
        bullet.transform.localScale = Vector3.one * 0.2f;

        var bulletRenderer = bullet.GetComponent<Renderer>();
        bulletRenderer.material = bulletMat; 

        var rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = firePoint.forward * bulletSpeed; // DÜZELTME

        var col = bullet.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        var bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.SetBulletProperties(bulletPower, bulletRange);

        if (bulletSFX != null)
            bulletSFXSource.PlayOneShot(bulletSFX);
    }

    private void ApplyVariantBaseStats()
    {
        baseFireRate    = fireRate;
        baseBulletPower = bulletPower;
        baseBulletRange = bulletRange;
        baseBulletSpeed = bulletSpeed;

        if (EvolutionState.Instance != null && EvolutionState.Instance.CurrentGunVariant == GunVariant.Evolved)
        {
            baseFireRate    = evolvedFireRate;
            baseBulletPower = evolvedBulletPower;
            baseBulletRange = evolvedBulletRange;
            baseBulletSpeed = evolvedBulletSpeed;

            fireRate    = baseFireRate;
            bulletPower = baseBulletPower;
            bulletRange = baseBulletRange;
            bulletSpeed = baseBulletSpeed;

            Debug.Log("[GunShooting] Evolved baz istatistikler uygulandı.");
        }
    }

    public void ReapplyVariantBaseStatsAndBonus()
    {
        ApplyVariantBaseStats();
        float bonus = WeaponPartInventory.Instance != null ? WeaponPartInventory.Instance.GetEquippedPartBonus() : 0f;
        ApplyBonus(bonus);
    }
}
