using UnityEngine;
using UnityEngine.InputSystem;

public class XRShipMovement : MonoBehaviour
{
    public GlobalVRControls controls; // Reference to GlobalVRControls
    public float movementSpeed = 2.0f; // Speed of the ship
    public float rotationSpeed = 100.0f; // Rotation speed of the ship
    private Rigidbody rb;

    private Vector2 movementInput; // Stores movement input values
    private Vector2 rotationInput; // Stores rotation input values

    private void Start()
    {
        // Ensure the ship is NOT parented to PlanetContainer or AsteroidContainer
        if (transform.parent != null)
        {
            Debug.LogWarning("Ship is parented to " + transform.parent.name + ". Detaching to prevent unwanted movement.");
            transform.parent = null; // Remove parent to stop inheriting transformations
        }

        // Ensure Rigidbody isn't causing unwanted rotation
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;  // Prevents external forces from rotating the ship
            rb.angularDamping = 1.0f; // FIX: Use angularDamping instead of angularDrag
        }
    }

    private void Update()
    {
        // Get movement and rotation input from VR controllers
        movementInput = controls.GetMovementInput();
        rotationInput = controls.GetRotationInput();

        // Handle movement and rotation based on VR input
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (movementInput.magnitude > 0.1f) // Ignore small movements
        {
            Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y) * movementSpeed * Time.deltaTime;
            transform.Translate(moveDirection, Space.Self);
        }
    }

    private void HandleRotation()
    {
        if (rotationInput.magnitude > 0.1f) // Ignore small rotation inputs
        {
            float rotationX = rotationInput.x * rotationSpeed * Time.deltaTime;
            float rotationY = -rotationInput.y * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, rotationX, Space.World);
            transform.Rotate(Vector3.right, rotationY, Space.World);

            // **Fix: Reset Rotation Input to Prevent Unwanted Continuous Rotation**
            rotationInput = Vector2.zero;
        }
    }
}
