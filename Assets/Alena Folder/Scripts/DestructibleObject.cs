using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public GameObject destroyEffect; // Optional visual effect prefab (e.g. particles)
    public AudioClip destroySound;   // Audio clip to play on destruction

    private bool isDestroyed = false;

    void OnCollisionEnter(Collision collision)
    {
        // Ignore if already destroyed or hit by something else
        if (isDestroyed)
            return;

        // Only respond to your actual bullets
        if (collision.gameObject.CompareTag("Player Bullet") || collision.gameObject.CompareTag("Laser"))
        {
            isDestroyed = true;

            // Optional VFX
            if (destroyEffect != null)
                Instantiate(destroyEffect, transform.position, Quaternion.identity);

            // Only play sound when hit — not on load
            if (destroySound != null)
                AudioSource.PlayClipAtPoint(destroySound, transform.position);

            // Destroy this object after the hit
            Destroy(gameObject);
        }
    }
}
