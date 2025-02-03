using UnityEngine;
using TMPro;

public class PlanetInteraction : MonoBehaviour
{
    public GameObject questionPanel; // Assign in Inspector
    private bool playerNearby = false;

    void Update()
    {
        // Detect player pressing X (VR trigger can be used here)
        if (playerNearby && Input.GetKeyDown(KeyCode.X))  // Replace with VR button later if needed
        {
            questionPanel.SetActive(true); // Show question panel when player interacts
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the spaceship has the "Player" tag
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            questionPanel.SetActive(false); // Hide question panel when the player leaves the area
        }
    }
}
