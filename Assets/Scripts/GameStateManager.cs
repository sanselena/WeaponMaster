using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    // Sahneler arasý taþýnacak veriler
    public bool HasPendingPresent { get; set; } = false;
    public int LastCompletedLevelIndex { get; set; } = -1;

    private void Awake()
    {
        // Singleton desenini uygula
        if (Instance != null && Instance != this)
        {
            // Zaten bir örnek varsa, bu yenisini yok et.
            Destroy(gameObject);
        }
        else
        {
            // Bu, tek örnek olarak ayarla.
            Instance = this;
            // Sahneler arasýnda yok olmamasýný saðla.
            DontDestroyOnLoad(gameObject);
        }
    }
}

