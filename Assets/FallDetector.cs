using UnityEngine;
using UnityEngine.UI;

public class FallDetector : MonoBehaviour
{
    public GameObject fallMessagePanel; // assign your UI panel in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // assuming your player has the tag "Player"
        {
            Debug.Log("Player fell off!");
            fallMessagePanel.SetActive(true);
        }
    }
}

