using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("GameObject References")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject bulletSpawner;


    [Header("Enemy Atributes")]
    [SerializeField] int health = 20;
    [SerializeField] float sightrange; // range of sight check
    [SerializeField] LayerMask playerLayermask; // layer the player is on
    bool playerInSight; // Holds if player is in sight

    // Update is called once per frame
    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightrange, playerLayermask); // Checks if player is in range
        if (playerInSight)
        {
            transform.LookAt(player.transform); // Turn enemy to player
            bulletSpawner.GetComponent<EnemyBulletSpawner>().SpawnBullet(); // Spawn a bullet and fire at player
        }
    }

    // When hit by something
    private void OnCollisionEnter(Collision collision)
    {
        // If a bullet hits the player, destroy the bullet and reduce health
        if (collision.gameObject.tag == "Player Bullet")
        {
            Destroy(collision.gameObject);
            health -= 10;
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
