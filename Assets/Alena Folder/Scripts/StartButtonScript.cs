using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StartButtonScript : MonoBehaviour
{
    public SceneManager sceneManager;

    private void Start()
    {
        // Add XR Interaction event listener
        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnInteract);
        }
    }

    public void OnInteract(SelectEnterEventArgs args)
    {
        Debug.Log("Start Button Pressed!");
        StartGame();
    }

    public void StartGame()
    {
        if (sceneManager != null)
        {
            sceneManager.CodexLevelPressed();
        }
        else
        {
            Debug.LogError("SceneManager is not assigned!");
        }
    }
}
