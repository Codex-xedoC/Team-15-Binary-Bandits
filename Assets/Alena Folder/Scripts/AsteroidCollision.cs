using UnityEngine;

public class AsteroidCollision : MonoBehaviour
{
    public int asteroidDamage = 5; // Each asteroid hit deducts 5 health

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            XRShipHealth.Instance.TakeDamage(asteroidDamage);
            Debug.Log("Player hit an asteroid! -5 Health");

        }
    }
}
