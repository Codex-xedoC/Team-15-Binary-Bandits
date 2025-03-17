using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class StartButtonScript : MonoBehaviour
{
    private SceneManager sceneManager;
    public string sceneToLoad = "CodexScene";

    public GameObject startButtonPanel; // UI Panel for Start Button
    public GameObject environmentParent; // Environment Parent (Planets/Asteroids)
    public GameObject playerShip; // Assign Player's Ship for movement

    public InputActionReference interactAction; // XR/Keyboard action reference

    private XRShipMovement shipMovementScript;
    private bool gameStarted = false; // Prevents multiple triggers

    void Start()
    {
        // Ensure GameObject is active before assigning input listeners
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("StartButtonScript is on an inactive object! Activating...");
            gameObject.SetActive(true);
        }

        // Find SceneManager
        sceneManager = FindObjectOfType<SceneManager>();

        if (sceneManager == null)
        {
            Debug.LogError("SceneManager not found! Ensure it exists in the hierarchy.");
        }

        // Get ship movement script
        if (playerShip != null)
        {
            shipMovementScript = playerShip.GetComponent<XRShipMovement>();
        }
        else
        {
            Debug.LogError("Player Ship not assigned in StartButtonScript!");
        }

        // Enable Input Action for Trigger/Click
        if (interactAction != null)
        {
            interactAction.action.performed += ctx => StartGame();
        }
        else
        {
            Debug.LogError("Interact Action not assigned!");
        }
    }

    public void StartGame()
    {
        if (gameStarted) return; // Prevent multiple activations
        gameStarted = true;

        Debug.Log("Start Button Pressed - Fading to black...");

        // Keep Start Button UI Visible Until Fade is Done
        StartCoroutine(FadeThenLoad());
    }

    IEnumerator FadeThenLoad()
    {
        // Start fade-out (keep UI active for now)
        if (sceneManager != null)
        {
            sceneManager.fadeScreen.FadeOut();
            yield return new WaitForSeconds(sceneManager.fadeScreen.fadeDuration);
        }

        // Hide Start Button AFTER fade completes
        if (startButtonPanel != null)
        {
            startButtonPanel.SetActive(false);
        }

        // Ensure environment loads
        if (environmentParent != null)
        {
            environmentParent.SetActive(true);
            Debug.Log("Environment enabled.");
        }
        else
        {
            Debug.LogWarning("Environment Parent not assigned! Planets/Asteroids might not appear.");
        }

        // Enable Ship Movement
        if (shipMovementScript != null)
        {
            shipMovementScript.enabled = true;
            Debug.Log("Movement script enabled.");
        }
        else
        {
            Debug.LogError("Ship movement script missing!");
        }

        // Start fade-in
        if (sceneManager != null)
        {
            sceneManager.fadeScreen.FadeIn();
            yield return new WaitForSeconds(sceneManager.fadeScreen.fadeDuration);
        }

        Debug.Log("Game started successfully!");
    }

    void OnDestroy()
    {
        // Unsubscribe from input action when destroyed
        if (interactAction != null)
        {
            interactAction.action.performed -= ctx => StartGame();
        }
    }
}
