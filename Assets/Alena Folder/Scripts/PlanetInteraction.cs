using UnityEngine;
using UnityEngine.InputSystem;

public class PlanetInteraction : MonoBehaviour
{
    public QuestionHelperCodex questionHelper; // Reference to the Question system
    public GameObject questionPanel; // UI Panel for displaying questions
    public InputActionProperty interactAction; // Assign the XR Controller Trigger

    private bool isNearPlanet = false;

    void Start()
    {
        if (questionPanel != null)
            questionPanel.SetActive(false);
    }

    void Update()
    {
        // Check if player is near a planet and presses the trigger button
        if (isNearPlanet && interactAction.action.WasPressedThisFrame())
        {
            ShowQuestion();
        }
    }

    private void ShowQuestion()
    {
        if (questionPanel != null && questionHelper != null)
        {
            questionPanel.SetActive(true); // Show the question UI
            questionHelper.DisplayNewQuestion(); // Pull and display a new question
        }
        else
        {
            Debug.LogWarning("Question UI or Helper is missing in PlanetInteraction.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Detect if the player enters the planet’s trigger
        {
            isNearPlanet = true;
            Debug.Log("Player is near a planet! Press the trigger to interact.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Detect when the player leaves the planet’s trigger
        {
            isNearPlanet = false;
            if (questionPanel != null)
            {
                questionPanel.SetActive(false); // Hide question UI when flying away
            }
            Debug.Log("Player left the planet’s interaction zone.");
        }
    }
}
