using UnityEngine;
using TMPro;

public class GunStats : MonoBehaviour
{
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI bulletPowerText;

    private GunShooting gunShooting;

    void Awake()
    {
        gunShooting = GetComponent<GunShooting>();
    }

    void Start()
    {
        UpdateStatDisplays();
        if (WeaponPartInventory.Instance != null)
        {
            // DÜZELTME: Parametresiz olan 'OnEquippedChanged' event'ine abone oluyoruz.
            WeaponPartInventory.Instance.OnEquippedChanged += UpdateStatDisplays;
        }
    }

    void OnDestroy()
    {
        if (WeaponPartInventory.Instance != null)
        {
            // DÜZELTME: Aboneliði ayný metoddan kaldýrýyoruz.
            WeaponPartInventory.Instance.OnEquippedChanged -= UpdateStatDisplays;
        }
    }

    // DÜZELTME: Bu metod artýk doðrudan event'e baðlanýyor ve parametre almýyor.
    private void UpdateStatDisplays()
    {
        float bonus = 0;
        if (WeaponPartInventory.Instance != null)
        {
            // Bonus, envanterin kendisinden alýnýyor.
            bonus = WeaponPartInventory.Instance.GetEquippedBonus();
        }
        
        Debug.Log("Gun stats updated due to equipment change. Bonus: " + bonus);
        
        // Stat'larý güncelleyen mevcut mantýðýnýzý buraya ekleyin.
        // Örneðin:
        // gunShooting.ApplyBonus(bonus);
        // fireRateText.text = $"Fire Rate: {gunShooting.currentFireRate}";
    }
}

