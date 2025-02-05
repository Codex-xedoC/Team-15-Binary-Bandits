using UnityEngine;
using UnityEngine.InputSystem;

public class XRShipMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f;

    [Header("Input Actions")]
    public InputActionProperty moveInput;  // Assign "Move" from GlobalVRControls in Unity

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ? Restore Rigidbody settings from when it worked before
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.linearDamping = 2f;  // Prevents excessive drifting
        rb.angularDamping = 5f;  // Prevents unwanted spin
    }

    void FixedUpdate()
    {
        if (moveInput.action == null)
        {
            Debug.LogWarning("? Move Input is NOT assigned! Assign it in Unity.");
            return;
        }

        Vector2 move = moveInput.action.ReadValue<Vector2>();

        if (move.sqrMagnitude > 0.01f) // Only move if joystick input detected
        {
            Vector3 moveDirection = (transform.forward * move.y) + (transform.right * move.x);
            rb.linearVelocity = moveDirection * speed;  
        }
        else
        {
            rb.linearVelocity = Vector3.zero;  //  Stops movement when joystick is idle
        }
    }
}
