using System;
using UnityEngine;

public enum GunVariant
{
    Base = 0,
    Evolved = 1
}

public class EvolutionState : MonoBehaviour
{
    public static EvolutionState Instance { get; private set; }

    public bool EvolutionUnlocked { get; private set; } = false;
    public GunVariant CurrentGunVariant { get; private set; } = GunVariant.Base;

    public event Action<GunVariant> OnVariantChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance == null)
        {
            var go = new GameObject("EvolutionState");
            go.AddComponent<EvolutionState>();
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[EvolutionState] Created and DontDestroyOnLoad.");
        }
        else if (Instance != this)
        {
            Debug.LogWarning("[EvolutionState] Duplicate detected. Destroying this one.");
            Destroy(gameObject);
        }
    }

    public void UpdateUnlockProgressOnLevelComplete()
    {
        var inv = WeaponPartInventory.Instance;
        var equipped = inv != null ? inv.GetEquippedHat() : WeaponPartId.None;

        Debug.Log($"[EvolutionState] LevelComplete check. EquippedHat={equipped}");
        if (equipped == WeaponPartId.PPAP)
        {
            EvolutionUnlocked = true;
            Debug.Log("[EvolutionState] EvolutionUnlocked = true (PPAP ile level tamamlandý)");
        }
    }

    public bool CanShowEvolutionButton()
    {
        return EvolutionUnlocked && CurrentGunVariant == GunVariant.Base;
    }

    public void ApplyEvolution()
    {
        if (CurrentGunVariant == GunVariant.Evolved) return;

        CurrentGunVariant = GunVariant.Evolved;

        var inv = WeaponPartInventory.Instance;
        if (inv != null) inv.ClearAll();

        Debug.Log("[EvolutionState] Evolution uygulandý: Gun = Evolved, envanter temizlendi.");
        OnVariantChanged?.Invoke(CurrentGunVariant);
    }
}