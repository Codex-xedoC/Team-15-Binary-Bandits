using UnityEngine;

public class AsteroidCollision : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public Transform damageTextSpawnPoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            XRShipHealth.Instance.TakeDamage(5); // -5 health

            // Show damage text at spawn point
            if (damageTextPrefab != null && damageTextSpawnPoint != null)
            {
                GameObject dmgText = Instantiate(damageTextPrefab, damageTextSpawnPoint.position, Quaternion.identity);
                Destroy(dmgText, 2f); // Destroy after 2 seconds
            }
        }
    }
}
