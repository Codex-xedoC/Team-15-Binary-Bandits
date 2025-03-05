using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // Ensure this is included!

public class StartButtonScript : MonoBehaviour, IPointerClickHandler
{
    public Button startButton; // Assign in Inspector
    public string gameSceneName = "GameScene"; // Change to your actual game scene name

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("Start Button not assigned in Inspector!");
        }
    }

    public void StartGame()
    {
        if (Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            Debug.Log("Start Button Pressed! Loading Game...");
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError($"Scene '{gameSceneName}' not found. Ensure it is added in Build Settings.");
        }
    }

    // Handles mouse clicks on the button
    public void OnPointerClick(PointerEventData eventData)
    {
        StartGame();
    }
}
