using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlanetInteractionScript : MonoBehaviour
{
    private bool isPlayerNear = false;
    public QuestionHelperCodex questionHelper;

    void Start()
    {
        if (questionHelper == null)
        {
            questionHelper = FindObjectOfType<QuestionHelperCodex>();
        }
    }

    // Trigger VR Interaction (instead of KeyCode.Z)
    public void OnInteract()
    {
        if (isPlayerNear)
        {
            Debug.Log("? VR Interaction triggered on " + gameObject.name);
            questionHelper.ShowQuestionAtPlanet();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
