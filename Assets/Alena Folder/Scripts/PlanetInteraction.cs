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

    [Header("View Detection Settings")]
    public float viewThreshold = 0.97f;

    private bool hasPlayedAudio = false;
    private bool canTriggerQuestion = true;

    [Header("Question Trigger Settings")]
    public float exitDistanceThreshold = 20f;

    private void Update()
    {
        if (!hasPlayedAudio && player != null)
        {
            Vector3 toPlanet = (transform.position - player.transform.position).normalized;
            Vector3 playerForward = player.transform.forward;

            float dot = Vector3.Dot(playerForward, toPlanet);

            if (dot > viewThreshold)
            {
                if (planetFoundAudio != null)
                {
                    planetFoundAudio.Play();
                    Debug.Log("[PlanetInteraction] Target acquired sound played (planet in view).");
                }

                hasPlayedAudio = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && canTriggerQuestion)
        {
            if (questionHelperCodex != null)
            {
                questionHelperCodex.DisplayNewQuestion();
                canTriggerQuestion = false;
                Debug.Log("[PlanetInteraction] Triggered question display.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
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

        canTriggerQuestion = true;
        Debug.Log("[PlanetInteraction] Player is far enough. Question can now re-trigger.");
    }
}
