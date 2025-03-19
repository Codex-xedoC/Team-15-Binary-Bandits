
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class StartButtonScript : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject environmentObjects;
    public GameObject playerShip;
    public GameObject questionPanel;
    public FadeScreen fadeScreen;
    public InputActionReference interactAction;

    private XRShipMovement shipMovementScript;
    private bool gameStarted = false;

    void Start()
    {
        if (startPanel == null || environmentObjects == null || fadeScreen == null || questionPanel == null)
        {
            Debug.LogError("StartButtonScript: Missing required components in Inspector.");
            return;
        }

        startPanel.SetActive(true);
        environmentObjects.SetActive(false);
        questionPanel.SetActive(false);

        if (playerShip != null)
        {
            shipMovementScript = playerShip.GetComponent<XRShipMovement>();
            if (shipMovementScript == null)
            {
                Debug.LogError("StartButtonScript: XRShipMovement script not found.");
            }
        }
        else
        {
            Debug.LogError("StartButtonScript: Player Ship not assigned.");
        }

        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += ctx => StartGame();
        }
        else
        {
            Debug.LogError("StartButtonScript: Interact Action not assigned.");
        }
    }

    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        Debug.Log("Start Button Pressed - Fading to black...");
        StartCoroutine(FadeThenLoad());
    }

    IEnumerator FadeThenLoad()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        // **Forcefully disable the Start Panel after fade-out**
        if (startPanel != null)
        {
            startPanel.SetActive(false);
            Canvas.ForceUpdateCanvases(); // Ensures UI updates correctly
            Debug.Log("Start Panel hidden after fade.");
        }

        environmentObjects.SetActive(true);
        Debug.Log("Environment enabled.");

        if (shipMovementScript != null)
        {
            shipMovementScript.enabled = true;
            Debug.Log("Movement enabled.");
        }

        fadeScreen.FadeIn();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        Debug.Log("Game started successfully.");
    }

    void OnDestroy()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= ctx => StartGame();
        }
    }
}