using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class StartButtonScript : MonoBehaviour
{
    public GameObject startPanel; // Start Menu UI
    public GameObject environmentObjects; // Planets, UI, etc.
    public GameObject playerShip; // Player's spaceship
    public FadeScreen fadeScreen; // Fader Screen for smooth transition
    public InputActionReference interactAction; // Trigger input action

    private bool gameStarted = false;

    void Start()
    {
        if (startPanel == null || environmentObjects == null || fadeScreen == null)
        {
            Debug.LogError("StartButtonScript: Missing required components in Inspector.");
            return;
        }

        // Ensure start panel is visible at the start
        startPanel.SetActive(true);
        environmentObjects.SetActive(false);

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
        if (gameStarted) return; // Prevent multiple presses
        gameStarted = true;

        Debug.Log("Start Button Pressed - Fading to black...");
        StartCoroutine(FadeThenLoad());
    }

    IEnumerator FadeThenLoad()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        // Hide start panel and enable environment objects
        startPanel.SetActive(false);
        environmentObjects.SetActive(true);

        Debug.Log("Environment enabled.");

        // Ensure player movement is enabled
        playerShip.GetComponent<XRShipMovement>().enabled = true;
        Debug.Log("Movement script enabled.");

        fadeScreen.FadeIn();
    }
}
