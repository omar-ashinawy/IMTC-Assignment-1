using UnityEngine;

// ELEMENT 1: Moving cube with WASD/Arrow keys
// Attach this to the PlayerCube GameObject
public class PlayerCubeController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Trail Settings")]
    public GameObject trailSpherePrefab;
    public float trailSpawnInterval = 0.1f;
    public float sphereLifetime = 5f;
    
    [Header("Color Settings")]
    public bool syncColors = true; // true = all spheres same color, false = individual colors
    public float colorChangeSpeed = 1f;

    private float timeSinceLastSpawn = 0f;
    private Color currentTrailColor;

    void Start()
    {
        // Create a sphere prefab at runtime if one isn't assigned
        if (trailSpherePrefab == null)
        {
            trailSpherePrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            trailSpherePrefab.transform.localScale = Vector3.one * 0.3f;
            trailSpherePrefab.SetActive(false);
        }

        currentTrailColor = Random.ColorHSV();
    }

    void Update()
    {
        HandleMovement();
        UpdateTrailColor();
    }

    void HandleMovement()
    {
        // Get input from WASD or Arrow keys
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down arrows

        Vector3 movement = new Vector3(horizontal, 0, vertical);

        // Only spawn trail if the cube is moving
        if (movement.magnitude > 0.1f)
        {
            // Normalize to prevent faster diagonal movement
            movement = movement.normalized;
            
            // Move the cube
            transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

            // Spawn trail spheres
            timeSinceLastSpawn += Time.deltaTime;
            if (timeSinceLastSpawn >= trailSpawnInterval)
            {
                SpawnTrailSphere();
                timeSinceLastSpawn = 0f;
            }
        }
    }

    void UpdateTrailColor()
    {
        // Continuously cycle through colors using HSV
        float hue = (Time.time * colorChangeSpeed * 0.1f) % 1f;
        currentTrailColor = Color.HSVToRGB(hue, 0.8f, 1f);
    }

    void SpawnTrailSphere()
    {
        // Instantiate a sphere at the cube's current position
        GameObject sphere = Instantiate(trailSpherePrefab, transform.position, Quaternion.identity);
        sphere.SetActive(true);

        // Get or add renderer
        Renderer renderer = sphere.GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = sphere.AddComponent<MeshRenderer>();
        }

        // Set color
        if (syncColors)
        {
            // All spheres will update to current color via TrailSphere script
            sphere.AddComponent<TrailSphere>().Initialize(sphereLifetime, this);
        }
        else
        {
            // Each sphere gets a unique random color
            Color uniqueColor = Random.ColorHSV();
            renderer.material.color = uniqueColor;
            sphere.AddComponent<TrailSphere>().Initialize(sphereLifetime, null);
        }

        // Destroy after lifetime
        Destroy(sphere, sphereLifetime);
    }

    public Color GetCurrentTrailColor()
    {
        return currentTrailColor;
    }
}

// Helper script for trail spheres
public class TrailSphere : MonoBehaviour
{
    private float lifetime;
    private PlayerCubeController controller;
    private Renderer sphereRenderer;

    public void Initialize(float life, PlayerCubeController ctrl)
    {
        lifetime = life;
        controller = ctrl;
        sphereRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // If synced colors, update to match controller's current color
        if (controller != null)
        {
            sphereRenderer.material.color = controller.GetCurrentTrailColor();
        }
    }
}