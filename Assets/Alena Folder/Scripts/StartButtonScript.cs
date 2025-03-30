using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StartButtonScript : XRBaseInteractable
{
    public GameObject startPanel;
    public GameObject environmentObjects;
    public GameObject playerShip;
    public GameObject questionPanel;
    public FadeScreen fadeScreen;

    private XRShipMovement shipMovementScript;
    private bool gameStarted = false;

    protected override void Awake()
    {
        base.Awake();

        if (startPanel == null || environmentObjects == null || fadeScreen == null || questionPanel == null)
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
            environmentObjects.SetActive(false); // Keep planets hidden until Start clicked
        }

        if (questionPanel != null)
        {
            questionPanel.SetActive(false); // Question UI stays hidden
        }

        // DO NOT disable ship movement — movement starts immediately
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

        // Hide startup UI
        if (startPanel != null)
        {
            startPanel.SetActive(false);
            Canvas.ForceUpdateCanvases();
            Debug.Log("Start panel hidden.");
        }

        // Enable gameplay environment (planets, questions)
        if (environmentObjects != null)
        {
            environmentObjects.SetActive(true);
            Debug.Log("Environment objects enabled.");
        }

        // Ship movement is already enabled before this — do not re-enable here

        fadeScreen.FadeIn();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        Debug.Log("Game started successfully.");
    }
}
