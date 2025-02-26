using UnityEngine;
using UnityEngine.SceneManagement;  // ? Ensure this is included
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StartButtonScript : MonoBehaviour
{
    public GameObject startButton;
    public GameObject planetContainer;

    private XRSimpleInteractable interactable;

    void Start()
    {
        interactable = startButton.GetComponent<XRSimpleInteractable>();

        if (interactable == null)
        {
            Debug.LogWarning("No XRSimpleInteractable found on the Start Button.");
        }
        else
        {
            interactable.selectEntered.AddListener(OnButtonClicked);
        }

        if (planetContainer != null)
        {
            DontDestroyOnLoad(planetContainer);
        }
        else
        {
            Debug.LogWarning("PlanetContainer not assigned.");
        }
    }

    private void OnButtonClicked(SelectEnterEventArgs args)
    {
        Debug.Log("Start button clicked! Attempting to load scene: CodexScene");

        try
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CodexScene");  // ? Force explicit reference
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load scene: " + e.Message);
        }
    }
}
