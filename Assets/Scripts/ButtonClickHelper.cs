using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Helper script to debug and fix button click issues
/// Add this to any button that's not working
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonClickHelper : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            // Ensure button is interactable
            button.interactable = true;
            
            // Check if EventSystem exists
            if (EventSystem.current == null)
            {
                Debug.LogError("No EventSystem found! Buttons won't work. EventSystem should be created automatically with Canvas.");
            }
            
            // Add click listener for debugging
            button.onClick.AddListener(() => {
                Debug.Log($"Button '{gameObject.name}' was clicked!");
            });
        }
    }
}

