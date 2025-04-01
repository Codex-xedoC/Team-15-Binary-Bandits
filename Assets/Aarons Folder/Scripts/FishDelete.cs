using UnityEngine;

public class FishDelete : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is tagged as "fish"
        if (other.CompareTag("fish"))
        {
            // Destroy the fish GameObject
            Destroy(other.gameObject);
        }
    }
}
