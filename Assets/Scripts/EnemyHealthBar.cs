using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public Vector3 offset = new Vector3(0, 2.5f, 0);

    private Enemy enemy;
    private Canvas canvas;
    private Slider slider;
    private GameObject barObject;
    private bool isVisible = false;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("EnemyHealthBar requires an Enemy component!");
            enabled = false;
            return;
        }

        CreateHealthBar();
        barObject.SetActive(false);
    }

    void CreateHealthBar()
    {
        // Root Canvas
        barObject = new GameObject("HealthBar");
        canvas = barObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        var scaler = barObject.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;

        var rectTransform = barObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 20);

        // Slider
        GameObject sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(barObject.transform, false);
        slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        slider.interactable = false;
        slider.direction = Slider.Direction.LeftToRight;

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        slider.targetGraphic = bgImage;

        // Fill Area
        GameObject fillAreaObj = new GameObject("Fill Area");
        fillAreaObj.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0);
        fillAreaRect.anchorMax = new Vector2(1, 1);
        fillAreaRect.sizeDelta = new Vector2(-4, -4); // Padding

        // Fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillAreaObj.transform, false);
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = Color.green;
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        // Assign fill
        slider.fillRect = fillRect;

        // Layout
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = Vector2.zero;
        sliderRect.anchorMax = Vector2.one;
        sliderRect.sizeDelta = Vector2.zero;

        // Set initial scale for world space
        barObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

    void LateUpdate()
    {
        if (!isVisible || barObject == null || enemy == null) return;
        barObject.transform.position = enemy.GetColliderPosition() + offset;
        if (Camera.main != null)
        {
            barObject.transform.LookAt(Camera.main.transform);
            barObject.transform.Rotate(0, 180, 0);
        }
    }

    public void Show()
    {
        if (barObject != null && !isVisible)
        {
            barObject.SetActive(true);
            isVisible = true;
        }
    }

    public void UpdateHealth()
    {
        if (slider != null && enemy != null)
        {
            float healthPercent = enemy.GetHealthPercent();
            slider.value = healthPercent;
        }
    }

    public void Hide()
    {
        if (barObject != null && isVisible)
        {
            barObject.SetActive(false);
            isVisible = false;
        }
    }

    void OnDestroy()
    {
        if (barObject != null)
        {
            Destroy(barObject);
        }
    }
}