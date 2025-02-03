using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalVRControls : MonoBehaviour
{
    public InputAction Move;     // Movement (translation) action
    public InputAction Rotate;   // Rotation (formerly Look) action
    public InputAction Interact; // Interaction action
    public InputAction Crouch;   // Crouch action
    public InputAction Jump;     // Jump action
    public InputAction Sprint;   // Sprint action

    private void Awake()
    {
        Move = new InputAction("Move", binding: "<XRController>{LeftHand}/primary2DAxis");
        Rotate = new InputAction("Rotate", binding: "<XRController>{RightHand}/primary2DAxis");
        Interact = new InputAction("Interact", binding: "<XRController>{LeftHand}/trigger");
        Crouch = new InputAction("Crouch", binding: "<XRController>{LeftHand}/primaryButton"); // example of crouch
        Jump = new InputAction("Jump", binding: "<XRController>{RightHand}/primaryButton"); // example of jump
        Sprint = new InputAction("Sprint", binding: "<XRController>{LeftHand}/secondaryButton"); // example of sprint
    }

    // Enable all actions
    public void Enable()
    {
        Move.Enable();
        Rotate.Enable();
        Interact.Enable();
        Crouch.Enable();
        Jump.Enable();
        Sprint.Enable();
    }

    // Disable all actions
    public void Disable()
    {
        Move.Disable();
        Rotate.Disable();
        Interact.Disable();
        Crouch.Disable();
        Jump.Disable();
        Sprint.Disable();
    }

    // Get Movement Input
    public Vector2 GetMovementInput()
    {
        return Move.ReadValue<Vector2>();
    }

    // Get Rotation Input
    public Vector2 GetRotationInput()
    {
        return Rotate.ReadValue<Vector2>();
    }

    // Check if Interact Button is pressed
    public bool IsInteractPressed()
    {
        return Interact.ReadValue<float>() > 0.5f; // Trigger is pressed if value is > 0.5
    }
}
