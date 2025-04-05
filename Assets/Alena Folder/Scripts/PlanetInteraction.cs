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

    public static bool hasPlayedAudio = false;
    private bool canTriggerQuestion = true;
    public static bool questionIsBeingDisplayed = false;

    [Header("Question Trigger Settings")]
    public float exitDistanceThreshold = 50f;

    private void Update()
    {
        if (!canTriggerQuestion && !questionIsBeingDisplayed)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance > exitDistanceThreshold)
            {
                canTriggerQuestion = true;
                Debug.Log("[PlanetInteraction] Player moved far enough to allow new question.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && canTriggerQuestion && !questionIsBeingDisplayed)
        {
            if (questionHelperCodex != null)
            {
                questionHelperCodex.DisplayNewQuestion();

                questionIsBeingDisplayed = true; // Stops duplicate questions.
                SpaceshipAudioController.stopTargetAquiredSound = true; // Stops sound from playing a lot.

                canTriggerQuestion = false;
                Debug.Log("[PlanetInteraction] Triggered question display.");

                StartCoroutine(SlowAndRestoreMovement());
            }
        }
    }

    private IEnumerator SlowAndRestoreMovement()
    {
        XRShipMovement ship = player.GetComponent<XRShipMovement>();

        float originalSpeed = ship.moveSpeed;
        float originalVertical = ship.verticalSpeed;

        ship.moveSpeed = originalSpeed * 0.1f;
        ship.verticalSpeed = originalVertical * 0.1f;

        yield return new WaitForSeconds(3f);

        ship.moveSpeed = originalSpeed;
        ship.verticalSpeed = originalVertical;

        questionIsBeingDisplayed = false;
        SpaceshipAudioController.stopTargetAquiredSound = false;
    }
}
