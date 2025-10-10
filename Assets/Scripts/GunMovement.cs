using UnityEngine;
using UnityEngine.InputSystem;

public class GunMovement : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float sideSpeed = 3f; // Reduced for more subtle movement
    public float trackWidth = 2.5f; // Much narrower track like in the video
    
    void Update()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        
        // L/R movement - more subtle like steering
        if (Keyboard.current.aKey.isPressed)
        {
            transform.Translate(Vector3.left * sideSpeed * Time.deltaTime);
        }
        if (Keyboard.current.dKey.isPressed)
        {
            transform.Translate(Vector3.right * sideSpeed * Time.deltaTime);
        }
        
        // Keep gun on narrow track 
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -trackWidth/2, trackWidth/2);
        transform.position = pos;
    }
}
