using UnityEngine;

// ELEMENT 4: Physics-based bouncing ball
// Attach this to the BouncingBall GameObject
// IMPORTANT: Add a Rigidbody component to the BouncingBall in the Inspector
public class BouncingBall : MonoBehaviour
{
    [Header("Physics Settings")]
    public float bounceForce = 10f;
    public float bounceInterval = 2f;
    public bool continuousBounce = true;

    [Header("Material Settings")]
    public PhysicsMaterial bouncyMaterial;
    
    private Rigidbody rb;
    private float timeSinceLastBounce = 0f;
    private Renderer ballRenderer;
    private int bounceCount = 0;

    void Start()
    {
        // Get or add Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Get renderer
        ballRenderer = GetComponent<Renderer>();
        if (ballRenderer != null)
        {
            ballRenderer.material = new Material(ballRenderer.material);
        }

        // Create bouncy physics material if not assigned
        if (bouncyMaterial == null)
        {
            bouncyMaterial = new PhysicsMaterial("Bouncy");
            bouncyMaterial.bounciness = 0.9f;
            bouncyMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
            bouncyMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;
        }

        // Apply physics material
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.material = bouncyMaterial;
        }

        // Initial bounce
        ApplyBounceForce();
    }

    void Update()
    {
        if (continuousBounce)
        {
            timeSinceLastBounce += Time.deltaTime;

            // Apply upward force periodically
            if (timeSinceLastBounce >= bounceInterval)
            {
                ApplyBounceForce();
                timeSinceLastBounce = 0f;
            }
        }

        // Change color based on velocity
        if (ballRenderer != null)
        {
            float speed = rb.linearVelocity.magnitude;
            float hue = Mathf.Clamp01(speed / 20f); // Normalize speed to 0-1
            ballRenderer.material.color = Color.HSVToRGB(hue, 0.8f, 1f);
        }
    }

    void ApplyBounceForce()
    {
        // Apply upward force
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        
        // Add slight random horizontal force for variation
        Vector3 randomForce = new Vector3(
            Random.Range(-2f, 2f),
            0f,
            Random.Range(-2f, 2f)
        );
        rb.AddForce(randomForce, ForceMode.Impulse);

        bounceCount++;
        Debug.Log($"Ball bounced! Count: {bounceCount}");
    }

    void OnCollisionEnter(Collision collision)
    {
        // Visual feedback on collision
        if (ballRenderer != null)
        {
            ballRenderer.material.color = Random.ColorHSV();
        }

        // Play sound effect here if you have audio
        Debug.Log($"Ball collided with {collision.gameObject.name}");
    }

    // Allow manual bounce with spacebar
    void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
        {
            ApplyBounceForce();
        }
    }
}