using UnityEngine;
using UnityEngine.InputSystem;

public class XRShipMovement : MonoBehaviour
{
    public Transform shipTransform;
    public Transform xrOriginTransform; // XR Origin (Headset Parent)

    public InputActionReference moveAction;
    public InputActionReference rotateAction;
    public InputActionReference rightGripAction;
    public InputActionReference leftGripAction;

    public float moveSpeed = 20f;
    public float rotationSpeed = 30f;
    public float verticalSpeed = 10f;

    private Vector2 moveInput;
    private Vector2 rotateInput;
    private float rightGripValue;
    private float leftGripValue;

    void Update()
    {
        // Get joystick inputs
        moveInput = moveAction.action.ReadValue<Vector2>();
        rotateInput = rotateAction.action.ReadValue<Vector2>();
        rightGripValue = rightGripAction.action.ReadValue<float>();
        leftGripValue = leftGripAction.action.ReadValue<float>();

        // Move forward/backward and strafe left/right
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        shipTransform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // **Rotate ship left/right using right joystick**
        if (Mathf.Abs(rotateInput.x) > 0.1f)
        {
            shipTransform.Rotate(Vector3.up * rotateInput.x * rotationSpeed * Time.deltaTime);
        }

        // **Vertical movement (right grip = up, left grip = down)**
        if (rightGripValue > 0.1f)
        {
            shipTransform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
        }
        if (leftGripValue > 0.1f)
        {
            shipTransform.Translate(Vector3.down * verticalSpeed * Time.deltaTime);
        }
    }
}
