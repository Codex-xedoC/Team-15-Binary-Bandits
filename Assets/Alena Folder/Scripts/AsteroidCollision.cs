using UnityEngine;

public class AsteroidCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Asteroid hit the player. Dealing damage.");
            XRShipHealth.Instance.TakeDamage(5);
        }
    }
}
