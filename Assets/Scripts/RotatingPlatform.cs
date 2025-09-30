using UnityEngine;

// ELEMENT 2: Rotating platform that also pulses in scale
// Attach this to the RotatingPlatform GameObject
public class RotatingPlatform : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationSpeed = new Vector3(0, 50, 0); // Degrees per second

    [Header("Scale Settings")]
    public bool enableScaling = true;
    public float scaleSpeed = 2f;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    [Header("Color Settings")]
    public bool changeColor = true;
    public float colorChangeSpeed = 0.5f;

    private Vector3 originalScale;
    private Renderer platformRenderer;

    void Start()
    {
        originalScale = transform.localScale;
        platformRenderer = GetComponent<Renderer>();
        
        // Create a material instance to avoid changing all objects with same material
        if (platformRenderer != null)
        {
            platformRenderer.material = new Material(platformRenderer.material);
        }
    }

    void Update()
    {
        // Continuous rotation
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Pulsing scale effect
        if (enableScaling)
        {
            float scale = Mathf.Lerp(minScale, maxScale, 
                (Mathf.Sin(Time.time * scaleSpeed) + 1f) / 2f);
            transform.localScale = originalScale * scale;
        }

        // Color cycling
        if (changeColor && platformRenderer != null)
        {
            float hue = (Time.time * colorChangeSpeed * 0.1f) % 1f;
            platformRenderer.material.color = Color.HSVToRGB(hue, 0.6f, 0.9f);
        }
    }
}