using AlenaScripts;
using UnityEngine;

public class AsteroidCollision : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public Transform damageTextSpawnPoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Replace XRShipHealth with your correct health script
            XRShipHealth.Instance.TakeDamage(5); // -5 health

            if (damageTextPrefab != null && damageTextSpawnPoint != null)
            {
                GameObject dmgText = Instantiate(damageTextPrefab, damageTextSpawnPoint.position, Quaternion.identity);
                Destroy(dmgText, 2f);
            }
        }
    }
}
