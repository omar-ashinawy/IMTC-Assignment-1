using UnityEngine;

// Camera that smoothly follows the player cube
// Attach this to the Main Camera
public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Drag the PlayerCube here in Inspector

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 0.125f;
    public bool lookAtTarget = true;

    void LateUpdate()
    {
        if (target == null)
        {
            // Try to find the player cube automatically
            GameObject player = GameObject.Find("PlayerCube");
            if (player != null)
            {
                target = player.transform;
            }
            return;
        }

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly interpolate to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Optionally look at the target
        if (lookAtTarget)
        {
            transform.LookAt(target);
        }
    }
}