using UnityEngine;
using UnityEngine.UI;

public class PlanetInteraction : MonoBehaviour
{
    public GameObject questionPanel; // Assign the Question UI Panel in Inspector
    public Text questionText; // Assign UI Text to display questions
    private bool isNearPlanet = false;

    void Start()
    {
        if (questionPanel != null)
            questionPanel.SetActive(false);
    }

    void Update()
    {
        if (isNearPlanet && Input.GetKeyDown(KeyCode.X)) // Press X to interact
        {
            ShowQuestion();
        }
    }

    private void ShowQuestion()
    {
        if (questionPanel != null)
        {
            questionPanel.SetActive(true);
            questionText.text = "What is the correct answer for this planet?";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Planet"))
        {
            isNearPlanet = true;
            Debug.Log("Near a Planet! Press X to interact.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Planet"))
        {
            isNearPlanet = false;
            questionPanel.SetActive(false);
        }
    }
}
