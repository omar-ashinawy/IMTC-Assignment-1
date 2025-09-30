using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// Manages the welcome text and instructions UI
// Attach this to an empty GameObject called "UIManager"
public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI welcomeText;
    public TextMeshProUGUI instructionsText;
    public GameObject instructionsPanel;
    public TextMeshProUGUI toggleHintText;

    [Header("Settings")]
    public bool startWithInstructionsVisible = true;
    public KeyCode toggleKey = KeyCode.H;
    public float fadeSpeed = 5f;

    private bool instructionsVisible;
    private CanvasGroup instructionsCanvasGroup;

    void Start()
    {
        // Auto-find UI elements if not assigned
        SetupUIReferences();

        // Set initial visibility
        instructionsVisible = startWithInstructionsVisible;
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(instructionsVisible);
        }

        // Set up canvas group for fading (optional)
        if (instructionsPanel != null)
        {
            instructionsCanvasGroup = instructionsPanel.GetComponent<CanvasGroup>();
            if (instructionsCanvasGroup == null)
            {
                instructionsCanvasGroup = instructionsPanel.AddComponent<CanvasGroup>();
            }
        }

        UpdateToggleHint();
    }

    void Update()
    {
        // Toggle instructions with H key (or use new Input System)
        bool togglePressed = false;
        
        // Try new Input System first
        if (Keyboard.current != null)
        {
            togglePressed = Keyboard.current.hKey.wasPressedThisFrame;
        }
        else
        {
            // Fallback to legacy input
            togglePressed = Input.GetKeyDown(toggleKey);
        }

        if (togglePressed)
        {
            ToggleInstructions();
        }
    }

    void SetupUIReferences()
    {
        // Auto-find welcome text
        if (welcomeText == null)
        {
            GameObject welcomeObj = GameObject.Find("WelcomeText");
            if (welcomeObj != null)
            {
                welcomeText = welcomeObj.GetComponent<TextMeshProUGUI>();
            }
        }

        // Auto-find instructions text
        if (instructionsText == null)
        {
            GameObject instrObj = GameObject.Find("InstructionsText");
            if (instrObj != null)
            {
                instructionsText = instrObj.GetComponent<TextMeshProUGUI>();
            }
        }

        // Auto-find instructions panel
        if (instructionsPanel == null)
        {
            instructionsPanel = GameObject.Find("InstructionsPanel");
        }

        // Auto-find toggle hint
        if (toggleHintText == null)
        {
            GameObject hintObj = GameObject.Find("ToggleHintText");
            if (hintObj != null)
            {
                toggleHintText = hintObj.GetComponent<TextMeshProUGUI>();
            }
        }

        // Set default text content
        SetDefaultText();
    }

    void SetDefaultText()
    {
        if (welcomeText != null)
        {
            welcomeText.text = "INTERACTIVE ELEMENTS DEMO";
        }

        if (instructionsText != null)
        {
            instructionsText.text = BuildInstructionsText();
        }
    }

    string BuildInstructionsText()
    {
        return @"<b>HOW TO INTERACT:</b>

<b>Moving Cube Trail</b>
• WASD or Arrow Keys - Move the cube

<b>Rotating Platform</b>
• Rotates and pulses automatically

<b>Clickable Spheres</b>
• Left Click on spheres

<b>Bouncing Ball</b>
• Spacebar - Extra bounce

<b>Object Spawner</b>
• E Key or Click Button - Spawn objects";
    }

    public void ToggleInstructions()
    {
        instructionsVisible = !instructionsVisible;

        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(instructionsVisible);
        }

        UpdateToggleHint();
    }

    void UpdateToggleHint()
    {
        if (toggleHintText != null)
        {
            toggleHintText.text = instructionsVisible ? 
                "Press [H] to hide instructions" : 
                "Press [H] to show instructions";
        }
    }

    // Public method to update instructions dynamically
    public void SetInstructionsText(string newText)
    {
        if (instructionsText != null)
        {
            instructionsText.text = newText;
        }
    }

    // Public method to show/hide
    public void ShowInstructions(bool show)
    {
        instructionsVisible = show;
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(instructionsVisible);
        }
        UpdateToggleHint();
    }
}