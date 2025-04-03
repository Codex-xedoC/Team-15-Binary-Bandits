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

    public static bool hasPlayedAudio = false; // public static allows the questions to reset this without a refrence to the planet.
    private bool canTriggerQuestion = true;
    public static bool questionIsBeingDisplayed = false; // Used to make sure a new question is not generated before a player is done.

    [Header("Question Trigger Settings")]
    public float exitDistanceThreshold = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && canTriggerQuestion && !questionIsBeingDisplayed)
        {
            if (questionHelperCodex != null)
            {
                questionHelperCodex.DisplayNewQuestion();
                canTriggerQuestion = false;
                Debug.Log("[PlanetInteraction] Triggered question display.");

                player.GetComponent<XRShipMovement>().isMovementLocked = true; // Locks player movement when question pops up
                questionIsBeingDisplayed = true; // Stops duplicate questions.
                SpaceshipAudioController.stopTargetAquiredSound = true; // Stops sound from playing alot.
            }
        }
    }
}
