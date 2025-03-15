using UnityEngine;
using UnityEngine.InputSystem;

public class XRShipMovement : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 100f;

    private Rigidbody rb;
    private Vector3 moveDirection;

    // VR Input
    public InputActionProperty moveInput;
    public InputActionProperty rotateInput; // Uses GlobalVRInputManager

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ?? Ensure Camera Offset is Parented to PlayerShip
        Transform cameraOffset = transform.Find("Camera Offset");
        if (cameraOffset != null && cameraOffset.parent != transform)
        {
            cameraOffset.SetParent(transform);
            Debug.Log("Camera Offset parented to PlayerShip.");
        }
    }

    void Update()
    {
        // ?? Keyboard Movement (WASD)
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow

        // ?? VR Joystick Movement
        Vector2 vrMove = moveInput.action.ReadValue<Vector2>();

        // ?? Combine VR & Keyboard Movement
        moveDirection = new Vector3(moveX + vrMove.x, 0, moveZ + vrMove.y);
        moveDirection = transform.TransformDirection(moveDirection) * speed;

        // ?? Apply Movement
        rb.linearVelocity = moveDirection; // Updated from velocity to linearVelocity

        // ?? Handle Rotation Using GlobalVRInputManager
        Vector2 vrRotate = rotateInput.action.ReadValue<Vector2>(); // Right joystick for looking
        transform.Rotate(Vector3.up * vrRotate.x * rotationSpeed * Time.deltaTime);

        // ?? Mouse Look (Only if VR isn't active)
        if (!moveInput.action.enabled) // Allow mouse look when VR joystick isn't used
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up * mouseX);
            Camera.main.transform.Rotate(Vector3.right * mouseY);
        }
    }
}
