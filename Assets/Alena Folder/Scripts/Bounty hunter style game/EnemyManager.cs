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
    [SerializeField] float sightrange;
    [SerializeField] LayerMask playerLayermask;
    bool playerInSight;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightrange, playerLayermask); // Checks if player is in range
        if (playerInSight)
        {
            transform.LookAt(player.transform);
            Debug.Log("Fire");
            bulletSpawner.GetComponent<EnemyBulletSpawner>().SpawnBullet();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Player Bullet")
        {
            Destroy(collision.gameObject);
            health -= 10;
            if (health <= 0)
            {
                //End Game
                
                Destroy(gameObject);
            }
        }
    }
}
