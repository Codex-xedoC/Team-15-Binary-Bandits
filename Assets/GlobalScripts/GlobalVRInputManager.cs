using UnityEngine;

public class GlobalVRInputManager : MonoBehaviour
{
    public GlobalVRControls inputControls; // Reference to GlobalVRControls script

    void Start()
    {
        if (inputControls == null)
        {
            Debug.LogError("GlobalVRControls is not assigned in GlobalVRInputManager.");
            return;
        }

        // Enable all input actions from GlobalVRControls
        inputControls.Enable();
    }

    void Update()
    {
        // Handle movement, rotation, and interaction here
        HandleMovement();
        HandleRotation();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        // Access GetMovementInput from GlobalVRControls script
        Vector2 movementInput = inputControls.GetMovementInput();
        if (movementInput.magnitude > 0.1f)
        {
            // Handle ship movement based on input (e.g., moving forward/backward)
            // Example: translate ship along the x and y axes
            Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y);
            transform.Translate(movement * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        // Access GetRotationInput from GlobalVRControls script
        Vector2 rotationInput = inputControls.GetRotationInput();
        if (rotationInput.magnitude > 0.1f)
        {
            // Handle ship rotation based on input (e.g., rotating ship)
            float rotationAngle = rotationInput.x; // Assume rotation on the X axis for simplicity
            transform.Rotate(Vector3.up, rotationAngle * Time.deltaTime * 100); // Adjust the multiplier as needed
        }
    }

    private void HandleInteraction()
    {
        // Check for interactions (e.g., button press)
        if (inputControls.IsInteractPressed())
        {
            // Handle interactions, like clicking to answer questions
            Debug.Log("Interact button pressed. Perform the interaction action.");
            // Call relevant interaction logic here
        }
    }

    void OnDisable()
    {
        // Disable all input actions when the game is disabled or quit
        inputControls.Disable();
    }
}
