using UnityEngine;

public class AsteroidCollision : MonoBehaviour
{
    public int damage = 5; // Amount of damage per asteroid collision
    public HealthUI healthUI; // Reference to HealthUI script

    private void Start()
    {
        if (healthUI == null)
        {
            Debug.LogError("HealthUI is not assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure spaceship has "Player" tag
        {
            healthUI.DecreaseHealth(damage); // Decrease health when colliding with asteroid
            Debug.Log("Asteroid hit! Health reduced.");
        }
    }
}
