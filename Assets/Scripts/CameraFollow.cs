using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -8); // Closer to the player
    
    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            
            // Look down at the target with an angle
            transform.LookAt(target);
        }
    }
}
