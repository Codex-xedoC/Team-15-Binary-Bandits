using UnityEngine;
using System.Collections.Generic;

public class SpaceshipAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource engineSource;
    public AudioSource effectSource;

    [Header("Sounds")]
    public AudioClip backgroundEngineSound;
    public AudioClip targetAcquiredSound;
    public AudioClip buttonPress;
    public AudioClip teleportSound;

    public static bool stopTargetAquiredSound = false;

    [Header("XR References")]
    public Camera xrOriginCamera;

    private string lastDetectedPlanet = "";
    private Dictionary<string, float> planetPingTimestamps = new Dictionary<string, float>();

    [Header("Detection Settings")]
    public float reacquireDistance = 100f;
    public float reacquireCooldown = 10f;
    public float detectionRange = 100f;

    void Start()
    {
        Debug.Log("[SpaceshipAudioController] Start() called");

        if (engineSource == null)
        {
            Debug.LogError("[SpaceshipAudioController] engineSource is NOT assigned");
        }
        if (effectSource == null)
        {
            Debug.LogError("[SpaceshipAudioController] effectSource is NOT assigned");
        }
        if (backgroundEngineSound == null)
        {
            Debug.LogError("[SpaceshipAudioController] backgroundEngineSound is NOT assigned");
        }

        if (engineSource != null && backgroundEngineSound != null)
        {
            engineSource.clip = backgroundEngineSound;
            engineSource.loop = true;
            engineSource.playOnAwake = false;
            engineSource.Play();
            Debug.Log("[SpaceshipAudioController] background engine sound started");
        }

        if (xrOriginCamera == null)
        {
            Debug.LogWarning("[SpaceshipAudioController] xrOriginCamera is not assigned — target acquired may not work");
        }
    }

    void Update()
    {
        DetectPlanetInView();
    }

    void DetectPlanetInView()
    {
        if (xrOriginCamera == null)
            return;

        if (!stopTargetAquiredSound)
        {
            RaycastHit hit;
            Vector3 origin = xrOriginCamera.transform.position;
            Vector3 direction = xrOriginCamera.transform.forward;

            if (Physics.Raycast(origin, direction, out hit, detectionRange))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag("Planet"))
                {
                    string planetName = hitObject.name;

                    if (CanReacquirePlanet(planetName, hitObject.transform.position))
                    {
                        if (effectSource != null && targetAcquiredSound != null)
                        {
                            effectSource.PlayOneShot(targetAcquiredSound);
                            Debug.Log("[SpaceshipAudioController] Target Acquired: " + planetName);
                        }

                        lastDetectedPlanet = planetName;
                        planetPingTimestamps[planetName] = Time.time;
                    }
                }
            }
        }
    }

    bool CanReacquirePlanet(string planetName, Vector3 planetPosition)
    {
        if (!planetPingTimestamps.ContainsKey(planetName))
            return true;

        float timeSinceLastPing = Time.time - planetPingTimestamps[planetName];
        float distance = Vector3.Distance(xrOriginCamera.transform.position, planetPosition);

        return (distance > reacquireDistance && timeSinceLastPing > reacquireCooldown);
    }

    public void PlayButtonSound()
    {
        if (buttonPress != null && effectSource != null)
        {
            effectSource.PlayOneShot(buttonPress);
        }
    }

    public void PlayTeleportSound()
    {
        if (teleportSound != null && effectSource != null)
        {
            effectSource.PlayOneShot(teleportSound);
        }
    }
}
