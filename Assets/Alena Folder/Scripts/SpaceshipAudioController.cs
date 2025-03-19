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

    public Camera xrOriginCamera; // Assign XR Origin Main Camera in the Inspector

    private bool targetSoundPlayed = false;

    void Start()
    {
        // Check if audio sources are assigned
        if (engineSource == null || effectSource == null)
        {
            Debug.LogError("Audio sources are not assigned! Assign them in the Inspector.");
            return;
        }

        // Check if XR Origin Camera is assigned
        if (xrOriginCamera == null)
        {
            Debug.LogError("XR Origin Camera not assigned! Assign it in the Inspector.");
            return;
        }

        // Play idle engine sound
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
    }

    void DetectPlanetInView()
    {
        RaycastHit hit;
        if (Physics.Raycast(xrOriginCamera.transform.position, xrOriginCamera.transform.forward, out hit, 100f))
        {
            if (hit.collider.CompareTag("Planet") && !targetSoundPlayed)
            {
                effectSource.PlayOneShot(targetAcquiredSound);
                targetSoundPlayed = true;
                Invoke(nameof(ResetTargetSound), 2f);
            }
        }
    }

    void ResetTargetSound()
    {
        targetSoundPlayed = false;
    }

    public void PlayTeleport()
    {
        if (effectSource != null && teleportSound != null)
        {
            effectSource.PlayOneShot(teleportSound);
        }
        else
        {
            Debug.LogError("Effect source or teleport sound not assigned!");
        }
    }
}
