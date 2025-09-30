using UnityEngine;

// ELEMENT 3: Objects that change color and scale when clicked
// Attach this to ClickableSphere1, ClickableSphere2, ClickableSphere3
public class ClickableObject : MonoBehaviour
{
    [Header("Click Response")]
    public bool changeColor = true;
    public bool scaleUp = true;
    public bool playAnimation = true;

    [Header("Animation Settings")]
    public float scaleMultiplier = 1.5f;
    public float animationDuration = 0.3f;

    private Renderer objectRenderer;
    private Vector3 originalScale;
    private bool isAnimating = false;
    private float animationTimer = 0f;
    private Vector3 targetScale;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalScale = transform.localScale;
        targetScale = originalScale;

        // Create material instance
        if (objectRenderer != null)
        {
            objectRenderer.material = new Material(objectRenderer.material);
        }
    }

    void Update()
    {
        // Handle mouse click detection
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    OnClicked();
                }
            }
        }

        // Handle scale animation
        if (isAnimating)
        {
            animationTimer += Time.deltaTime;
            float progress = animationTimer / animationDuration;

            if (progress < 0.5f)
            {
                // Scale up
                float scaleProgress = progress * 2f;
                transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleProgress);
            }
            else
            {
                // Scale back down
                float scaleProgress = (progress - 0.5f) * 2f;
                transform.localScale = Vector3.Lerp(targetScale, originalScale, scaleProgress);
            }

            if (progress >= 1f)
            {
                isAnimating = false;
                transform.localScale = originalScale;
                animationTimer = 0f;
            }
        }
    }

    void OnClicked()
    {
        Debug.Log($"{gameObject.name} was clicked!");

        // Change to random color
        if (changeColor && objectRenderer != null)
        {
            Color newColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.7f, 1f);
            objectRenderer.material.color = newColor;
        }

        // Trigger scale animation
        if (scaleUp && playAnimation && !isAnimating)
        {
            targetScale = originalScale * scaleMultiplier;
            isAnimating = true;
            animationTimer = 0f;
        }
    }

    // Visual feedback: slight highlight on hover
    void OnMouseEnter()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.SetFloat("_Emission", 0.3f);
        }
    }

    void OnMouseExit()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.SetFloat("_Emission", 0f);
        }
    }
}