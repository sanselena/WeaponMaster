using UnityEngine;
using UnityEngine.UI;

public class TeleportingButton : MonoBehaviour
{
    public RectTransform buttonRectTransform; // Assign the button itself
    public Vector2[] teleportPositions;       // Assign positions in the Inspector
    public string[] buttonTexts;              // Set different texts for each teleport
    public float showDelay = 3f;
     

    private int clickCount = 0;
    private Button button;
    private Text buttonText;
    void Start()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();

        // Hide the button initially
        gameObject.SetActive(false);

        // Schedule to show after delay
        Invoke(nameof(ShowButton), showDelay);

        button.onClick.AddListener(OnButtonClick);
    }

    void ShowButton()
    {
        gameObject.SetActive(true);
        button.interactable = true;
        if (teleportPositions.Length > 0)
            buttonRectTransform.anchoredPosition = teleportPositions[0];
        if (buttonTexts.Length > 0 && buttonText != null)
            buttonText.text = buttonTexts[0];
    }

    void OnButtonClick()
    {
        clickCount++;

        if (clickCount < teleportPositions.Length)
        {
            buttonRectTransform.anchoredPosition = teleportPositions[clickCount];
            // Change button text
            if (buttonTexts.Length > clickCount && buttonText != null)
                buttonText.text = buttonTexts[clickCount];
        }
        else if (clickCount == teleportPositions.Length)
        {
                AudioListener.volume = 0; // For muting all audio

            gameObject.SetActive(false);
        }
    }
}