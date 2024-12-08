using UnityEngine;

public class BobAndRotate : MonoBehaviour
{
    public float bobbingSpeed = 1.5f;   // Speed of the bobbing effect
    public float bobbingHeight = 0.15f;  // Height difference for the bobbing effect
    public float rotationSpeed = 50f;   // Speed of the rotation in degrees per second

    private Vector3 initialPosition;
    private Quaternion initialRotation; // Store the initial orientation

    void Start()
    {
        // Store the initial position and rotation of the GameObject
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Bobbing up and down
        float newY = initialPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);

        // Only apply rotation to the Y-axis
        float newYRotation = initialRotation.eulerAngles.y + (rotationSpeed * Time.time);
        transform.rotation = Quaternion.Euler(initialRotation.eulerAngles.x, newYRotation, initialRotation.eulerAngles.z);
    }
}