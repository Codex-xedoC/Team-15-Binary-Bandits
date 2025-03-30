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
    public AudioSource engineSource; 

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

        if (engineSource == null)
        {
            Debug.LogWarning("StartButtonScript: Engine audio source not assigned.");
        }
    }

    void Start()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }

        if (environmentObjects != null)
        {
            environmentObjects.SetActive(false);
        }

        if (uiRoot != null)
        {
            uiRoot.SetActive(false);
        }

        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }
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
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        if (startPanel != null)
        {
            startPanel.SetActive(false);
            Canvas.ForceUpdateCanvases();
            Debug.Log("Start panel hidden.");
        }

        if (environmentObjects != null)
        {
            environmentObjects.SetActive(true);
            Debug.Log("Environment objects enabled.");
        }

        if (uiRoot != null)
        {
            uiRoot.SetActive(true);
            Debug.Log("UI root enabled.");
        }

        // Start the engine sound manually here
        if (engineSource != null && !engineSource.isPlaying)
        {
            engineSource.Play();
            Debug.Log("Engine audio started.");
        }

        fadeScreen.FadeIn();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        Debug.Log("Game started successfully.");
    }
}
