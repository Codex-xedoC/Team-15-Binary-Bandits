using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 10, 0); // Adjust values for rotation speed

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;  // Prevents physics-based rotation
            rb.useGravity = false;  // Ensures planets do not move due to gravity
        }
    }

    void Update()
    {
        // Rotate in place instead of orbiting
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }
}
