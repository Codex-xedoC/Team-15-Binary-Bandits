using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletSpawner : MonoBehaviour
{
    [Header("Object Refferences")]
    [SerializeField] GameObject bullet, player; // Gets a Refernece to the bullet and player Prefab

    [Header("Bullet Values")]
    [SerializeField] float lifeSpan; // how long until the bullet destroys
    [SerializeField] float bulletCooldown; // how long until the bullet destroys
    [SerializeField] float bulletSpeed = 100; // how fast the bullet moves
    bool bulletOnCooldown; // how long until the bullet destroys

    [Header("Input References")]
    [SerializeField] InputActionReference leftTrigerValue; // Left trigger input reference
    [SerializeField] InputActionReference rightTrigerValue; // Right trigger input reference

    [SerializeField] InputActionReference rightAbuttonValue; // DEBUG spawn with simulator

    // Update is called once per frame
    void Update()
    {
        if (leftTrigerValue.action.ReadValue<float>() == 1 && rightTrigerValue.action.ReadValue<float>() == 1) // if player holds both triggers, fire bullet
        {
            SpawnBullet();
        }
        if (rightAbuttonValue.action.ReadValue<float>() == 1) // DEBUG able to spawn on simulator
        {
            SpawnBullet();
        }
    }

    // Spawns in a bullet and sets time for it to destroy
    void SpawnBullet()
    {
        if (!player.GetComponent<XRShipMovement>().isMovementLocked && !bulletOnCooldown) // If player can move (or not in question) allow them to shoot
        {
            StartCoroutine(BulletCoolDownTimer()); // Start Cooldown

            GameObject bulletInst = Instantiate(bullet, transform.position, transform.rotation); // Spawn in bullet in position this object

            // Bullet movement
            Rigidbody bulRB = bulletInst.transform.GetChild(0).GetComponent<Rigidbody>(); // Get rigidbdy of bullet
            bulRB.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse); // Cause bullet to move once spawned

            Destroy(bulletInst, lifeSpan); // Destroyes bullet after set time
        }
    }

    // Sets bullet spawner on cooldown so player can not spam shoot
    IEnumerator BulletCoolDownTimer()
    {
        bulletOnCooldown = true;
        yield return new WaitForSeconds(bulletCooldown); // Wait for end of bullet cooldown
        bulletOnCooldown = false;
    }
}
