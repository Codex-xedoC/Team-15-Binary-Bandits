using UnityEngine;
using System.Collections;

public class PlanetInteraction : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject player;

    [Header("Audio")]
    public AudioSource planetFoundAudio;

    [Header("Question System")]
    public QuestionHelperCodex questionHelperCodex;

    private bool hasPlayedAudio = false;
    private bool canTriggerQuestion = true;

    [Header("Question Trigger Settings")]
    public float exitDistanceThreshold = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && canTriggerQuestion)
        {
            Debug.Log($"[PlanetInteraction] Player entered planet zone: {gameObject.name}");

            if (!hasPlayedAudio && planetFoundAudio != null)
            {
                planetFoundAudio.Play();
                hasPlayedAudio = true;
            }

            if (questionHelperCodex != null)
            {
                questionHelperCodex.DisplayNewQuestion();
                canTriggerQuestion = false;
                Debug.Log("[PlanetInteraction] Triggered question display.");
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
            Debug.Log($"[PlanetInteraction] Player exited planet zone: {gameObject.name}");
            hasPlayedAudio = false;
            StartCoroutine(CheckDistanceToRearm());
        }
    }

    private IEnumerator CheckDistanceToRearm()
    {
        Transform planet = this.transform;
        Transform playerTransform = player.transform;

        while (Vector3.Distance(playerTransform.position, planet.position) < exitDistanceThreshold)
        {
            yield return null;
        }

        Debug.Log("[PlanetInteraction] Player is far enough from planet. Question can now re-trigger.");
        canTriggerQuestion = true;
    }
}
