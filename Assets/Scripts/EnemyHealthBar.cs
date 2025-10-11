using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public Vector3 offset = new Vector3(0, 2.5f, 0); // Height above enemy
    public Vector2 size = new Vector2(1f, 0.15f); // Width and height of bar

    private Enemy enemy;
    private Canvas canvas;
    private Slider slider;
    private GameObject barObject;
    private bool isVisible = false;

    void Start()
    {
        // Get the Enemy component
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("EnemyHealthBar requires an Enemy component!");
            enabled = false;
            return;
        }

        // Create the health bar
        CreateHealthBar();

        // Hide until first damage
        barObject.SetActive(false);

        Debug.Log("Health bar created and hidden");
    }

    void CreateHealthBar()
    {
        // Create root GameObject
        barObject = new GameObject("HealthBar");

        // Add Canvas
        canvas = barObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        // Set canvas size
        RectTransform canvasRect = barObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(size.x * 100, size.y * 100); // Scale up for world space

        // Create background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(barObject.transform);

        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); // Dark semi-transparent

        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;

        // Create fill (health bar itself)
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(barObject.transform);

        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.green;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;

        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;

        // Create slider component for easy control
        slider = barObject.AddComponent<Slider>();
        slider.fillRect = fillRect;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        slider.interactable = false;

        // Set initial scale
        barObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        Debug.Log("Health bar UI created");
    }

    void LateUpdate()
    {
        if (!isVisible || barObject == null || enemy == null) return;

        // Position above enemy's collider
        barObject.transform.position = enemy.GetColliderPosition() + offset;

        // Face camera
        if (Camera.main != null)
        {
            barObject.transform.LookAt(Camera.main.transform);
            barObject.transform.Rotate(0, 180, 0);
        }
    }

    public void Show()
    {
        if (barObject != null)
        {
            barObject.SetActive(true);
            isVisible = true;
            Debug.Log("Health bar now visible");
        }
    }

    public void UpdateHealth()
    {
        if (slider != null && enemy != null)
        {
            float healthPercent = enemy.GetHealthPercent();
            slider.value = healthPercent;

            // Change color based on health
            Image fillImage = slider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                if (healthPercent > 0.5f)
                    fillImage.color = Color.green;
                else if (healthPercent > 0.25f)
                    fillImage.color = Color.yellow;
                else
                    fillImage.color = Color.red;
            }
        }
    }

    public void Hide()
    {
        if (barObject != null)
        {
            barObject.SetActive(false);
            isVisible = false;
        }
    }

    void OnDestroy()
    {
        // Clean up health bar when enemy is destroyed
        if (barObject != null)
        {
            Destroy(barObject);
        }
    }
}