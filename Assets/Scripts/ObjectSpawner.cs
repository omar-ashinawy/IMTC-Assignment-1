using UnityEngine;
using UnityEngine.UI;

// ELEMENT 5: UI Button that spawns objects in the scene
// Attach this to an empty GameObject (like TrailManager or create SpawnerManager)
public class ObjectSpawner : MonoBehaviour
{
    [Header("UI Settings")]
    public Button spawnButton; // Drag the button from Canvas here

    [Header("Spawn Settings")]
    public GameObject[] objectsToSpawn; // Prefabs or primitives
    public Transform spawnPoint; // Where objects spawn (can be the player)
    public float spawnHeight = 5f;
    public float spawnRadius = 3f;
    public float despawnTime = 10f;

    [Header("Spawn Effects")]
    public bool randomColor = true;
    public bool addPhysics = true;
    public bool randomRotation = true;

    private int spawnCount = 0;

    void Start()
    {
        // Find button automatically if not assigned
        if (spawnButton == null)
        {
            spawnButton = GameObject.Find("SpawnButton")?.GetComponent<Button>();
        }

        // Add listener to button
        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(SpawnObject);
            
            // Update button text if it has one
            Text buttonText = spawnButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = "Spawn Object";
            }
        }
        else
        {
            Debug.LogWarning("Spawn button not found! Create a Button in Canvas and assign it.");
        }

        // Set spawn point to player if not assigned
        if (spawnPoint == null)
        {
            GameObject player = GameObject.Find("PlayerCube");
            if (player != null)
            {
                spawnPoint = player.transform;
            }
            else
            {
                spawnPoint = transform;
            }
        }

        // Create default objects to spawn if none assigned
        if (objectsToSpawn == null || objectsToSpawn.Length == 0)
        {
            objectsToSpawn = new GameObject[3];
            objectsToSpawn[0] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            objectsToSpawn[1] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            objectsToSpawn[2] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            
            // Hide the templates
            foreach (GameObject obj in objectsToSpawn)
            {
                obj.SetActive(false);
            }
        }
    }

    public void SpawnObject()
    {
        if (objectsToSpawn.Length == 0) return;

        // Choose random object type
        GameObject prefab = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];

        // Calculate spawn position (above and around spawn point)
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0; // Keep on same height plane
        Vector3 spawnPosition = spawnPoint.position + randomOffset + Vector3.up * spawnHeight;

        // Calculate rotation
        Quaternion spawnRotation = randomRotation ? 
            Random.rotation : 
            Quaternion.identity;

        // Instantiate object
        GameObject spawnedObject = Instantiate(prefab, spawnPosition, spawnRotation);
        spawnedObject.SetActive(true);
        spawnedObject.name = $"SpawnedObject_{spawnCount}";

        // Apply random color
        if (randomColor)
        {
            Renderer renderer = spawnedObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(renderer.material);
                renderer.material.color = Random.ColorHSV();
            }
        }

        // Add physics
        if (addPhysics)
        {
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = spawnedObject.AddComponent<Rigidbody>();
            }
            
            // Add random initial force
            Vector3 randomForce = new Vector3(
                Random.Range(-3f, 3f),
                Random.Range(0f, 2f),
                Random.Range(-3f, 3f)
            );
            rb.AddForce(randomForce, ForceMode.Impulse);
        }

        // Destroy after time
        Destroy(spawnedObject, despawnTime);

        spawnCount++;
        Debug.Log($"Spawned object #{spawnCount}");

        // Update button text
        Text buttonText = spawnButton?.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = $"Spawn Object ({spawnCount})";
        }
    }

    // Alternative: spawn with keyboard key (E key)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnObject();
        }
    }
}