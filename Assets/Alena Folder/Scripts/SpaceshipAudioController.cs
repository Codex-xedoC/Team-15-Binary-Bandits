using UnityEngine;

public class SpaceshipAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource engineSource;
    public AudioSource effectSource;

    [Header("Spaceship Sounds")]
    public AudioClip spaceshipEngineIdle;
    public AudioClip spaceshipEngineThrust;
    public AudioClip buttonPress;
    public AudioClip teleportSound;
    public AudioClip targetAcquiredSound;

    [Header("References")]
    public Camera xrOriginCamera;
    public Rigidbody playerShipRigidbody; // To check movement for thrust sound

    private bool targetSoundPlayed = false;
    private string lastDetectedPlanet = "";
    private bool isThrusting = false;

    void Start()
    {
        if (engineSource == null || effectSource == null)
        {
            Debug.LogError("Audio sources not assigned! Assign them in the Inspector.");
            return;
        }

        if (xrOriginCamera == null)
        {
            Debug.LogError("XR Origin Camera not assigned! Assign it in the Inspector.");
            return;
        }

        if (playerShipRigidbody == null)
        {
            Debug.LogError("Player Ship Rigidbody not assigned! Assign it in the Inspector.");
            return;
        }

        // Start with idle engine sound
        engineSource.clip = spaceshipEngineIdle;
        engineSource.loop = true;
        engineSource.Play();
    }

    void Update()
    {
        if (xrOriginCamera != null)
        {
            DetectPlanetInView();
        }

        HandleEngineSound();
    }

    void DetectPlanetInView()
    {
        RaycastHit hit;
        int planetLayerMask = LayerMask.GetMask("Planet"); // Ensure planets are in the "Planet" layer

        if (Physics.Raycast(xrOriginCamera.transform.position, xrOriginCamera.transform.forward, out hit, 100f, planetLayerMask))
        {
            if (!targetSoundPlayed && hit.collider.CompareTag("Planet") && hit.collider.name != lastDetectedPlanet)
            {
                effectSource.PlayOneShot(targetAcquiredSound);
                lastDetectedPlanet = hit.collider.name;
                targetSoundPlayed = true;
                Invoke(nameof(ResetTargetSound), 5f);
            }
        }
    }

    void HandleEngineSound()
    {
        // Check if the ship is moving
        if (playerShipRigidbody.linearVelocity.magnitude > 0.1f)
        {
            if (!isThrusting)
            {
                engineSource.clip = spaceshipEngineThrust;
                engineSource.loop = true;
                engineSource.Play();
                isThrusting = true;
            }
        }
        else
        {
            if (isThrusting)
            {
                engineSource.clip = spaceshipEngineIdle;
                engineSource.loop = true;
                engineSource.Play();
                isThrusting = false;
            }
        }
    }

    void ResetTargetSound()
    {
        targetSoundPlayed = false;
        lastDetectedPlanet = "";
    }
}
