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

    [Header("XR References")]
    public Camera xrOriginCamera;

    private string lastDetectedPlanet = "";
    private Dictionary<string, float> planetPingTimestamps = new Dictionary<string, float>();
    private float reacquireDistance = 100f;
    private float reacquireCooldown = 10f;

    void Start()
    {
        Debug.Log("[SpaceshipAudioController] Start() called");

        if (engineSource == null)
        {
            Debug.LogError("engineSource is NOT assigned");
        }
        if (effectSource == null)
        {
            Debug.LogError("effectSource is NOT assigned");
        }
        if (backgroundEngineSound == null)
        {
            Debug.LogError("backgroundEngineSound is NOT assigned");
        }

        if (engineSource != null && backgroundEngineSound != null)
        {
            engineSource.clip = backgroundEngineSound;
            engineSource.loop = true;
            engineSource.playOnAwake = false;
            engineSource.Play();
            Debug.Log("backgroundEngineSound forced to play at Start()");
        }

        if (xrOriginCamera == null)
        {
            Debug.LogWarning("xrOriginCamera is not assigned — target acquired may not work");
        }
    }

    void Update()
    {
        DetectPlanetInView();

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (effectSource != null && targetAcquiredSound != null)
            {
                effectSource.PlayOneShot(targetAcquiredSound);
                Debug.Log("Manual test: Played targetAcquiredSound via P key");
            }
        }
    }

    void DetectPlanetInView()
    {
        if (xrOriginCamera == null) return;

        RaycastHit hit;
        Vector3 origin = xrOriginCamera.transform.position;
        Vector3 direction = xrOriginCamera.transform.forward;
        float maxDistance = 100f;

        if (Physics.Raycast(origin, direction, out hit, maxDistance))
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
                        Debug.Log("Target Acquired: " + planetName);
                    }

                    lastDetectedPlanet = planetName;
                    planetPingTimestamps[planetName] = Time.time;
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

        if (distance > reacquireDistance && timeSinceLastPing > reacquireCooldown)
        {
            return true;
        }

        return false;
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
