using UnityEngine;

public class PlanetInteraction : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject player;

    [Header("Audio")]
    public AudioSource planetFoundAudio;

    [Header("Question System")]
    public QuestionHelperCodex questionHelperCodex;

    private bool isPlayerNear = false;
    private bool hasPlayedAudio = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerNear = true;

            if (!hasPlayedAudio && planetFoundAudio != null)
            {
                planetFoundAudio.Play();
                hasPlayedAudio = true;
            }

            Debug.Log("[PlanetInteraction] Player entered planet zone, displaying random question.");
            if (questionHelperCodex != null)
            {
                questionHelperCodex.DisplayNewQuestion();
            }
            else
            {
                Debug.LogError("PlanetInteraction: QuestionHelperCodex not assigned.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerNear = false;
            hasPlayedAudio = false;
        }
    }
}
