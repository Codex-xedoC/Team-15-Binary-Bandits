using UnityEngine;

public class TutorialCheckpoint : MonoBehaviour
{
    [Tooltip("The tutorial UI card to show when entering this checkpoint.")]
    public GameObject tutorialCard;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || tutorialCard == null)
            return;

        // Make sure the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            triggered = true;
            tutorialCard.SetActive(true);

            // Optional: change cube color to show it's completed
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.green;
            }
        }
    }
}
