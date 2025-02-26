using UnityEngine;
using UnityEngine.InputSystem;

public class XRShipMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 300f;  // SIGNIFICANTLY faster
    public float acceleration = 50f;  // Added acceleration for smoother speed-up
    public float maxSpeed = 500f;  // Ship won't be clamped artificially

    [Header("Input Actions")]
    public InputActionProperty moveInput;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Enable smooth movement
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.linearDamping = 0.05f;  // Reduce drag so it moves freely
        rb.angularDamping = 1f;  // Less restrictive turning
    }

    void FixedUpdate()
    {
        if (moveInput.action == null)
        {
            Debug.LogWarning("? Move Input is NOT assigned! Assign it in Unity.");
            return;
        }

        Vector2 move = moveInput.action.ReadValue<Vector2>();

        if (move.sqrMagnitude > 0.01f)
        {
            Vector3 moveDirection = (transform.forward * move.y) + (transform.right * move.x);
            rb.AddForce(moveDirection * acceleration, ForceMode.Acceleration);

            // Cap speed so it doesn't go out of control
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            rb.linearVelocity *= 0.98f;  // Smooth deceleration instead of instant stop
        }
    }
}
