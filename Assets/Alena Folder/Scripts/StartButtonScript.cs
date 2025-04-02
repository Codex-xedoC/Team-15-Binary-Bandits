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
    public GameObject tutorialRoot; // ADD THIS in Inspector
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
            startPanel.SetActive(true);

        if (environmentObjects != null)
            environmentObjects.SetActive(false);

        if (questionPanel != null)
            questionPanel.SetActive(false);

        if (tutorialRoot != null)
            tutorialRoot.SetActive(true);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (!gameStarted && args.interactorObject.transform.name.Contains("RightHand"))
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
            startPanel.SetActive(false);

        if (tutorialRoot != null)
            tutorialRoot.SetActive(false);

        if (environmentObjects != null)
            environmentObjects.SetActive(true);

        if (questionPanel != null)
            questionPanel.SetActive(false);

        fadeScreen.FadeIn();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        Debug.Log("Game started.");
    }
}
