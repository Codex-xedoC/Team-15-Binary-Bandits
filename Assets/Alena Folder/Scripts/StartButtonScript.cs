using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StartButtonScript : XRBaseInteractable
{
    public GameObject startPanel;
    public GameObject environmentObjects;
    public GameObject uiRoot;             
    public GameObject playerShip;
    public GameObject questionPanel;      
    public FadeScreen fadeScreen;

    private XRShipMovement shipMovementScript;
    private bool gameStarted = false;

    protected override void Awake()
    {
        base.Awake();

        if (startPanel == null || environmentObjects == null || uiRoot == null || fadeScreen == null || questionPanel == null)
        {
            Debug.LogError("StartButtonScript: Missing required components in Inspector.");
        }

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
    }

    void Start()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(true); // Show Start UI when scene loads
        }

        if (environmentObjects != null)
        {
            environmentObjects.SetActive(false); // Disable planets/asteroids
        }

        if (uiRoot != null)
        {
            uiRoot.SetActive(false); // Disable UI canvas with question panels
        }

        if (questionPanel != null)
        {
            questionPanel.SetActive(false); // Make sure no questions show on load
        }

        // Ship movement is already active — don't disable it
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (!gameStarted)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        Debug.Log("Start button pressed. Beginning fade...");
        StartCoroutine(FadeThenLoad());
    }

    IEnumerator FadeThenLoad()
    {
        // Fade to black
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        // Hide start panel
        if (startPanel != null)
        {
            startPanel.SetActive(false);
            Canvas.ForceUpdateCanvases();
            Debug.Log("Start panel hidden.");
        }

        // Enable planets/asteroids
        if (environmentObjects != null)
        {
            environmentObjects.SetActive(true);
            Debug.Log("Environment objects enabled.");
        }

        // Enable question UI
        if (uiRoot != null)
        {
            uiRoot.SetActive(true);
            Debug.Log("UI root enabled.");
        }

        fadeScreen.FadeIn();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        Debug.Log("Game started successfully.");
    }
}
