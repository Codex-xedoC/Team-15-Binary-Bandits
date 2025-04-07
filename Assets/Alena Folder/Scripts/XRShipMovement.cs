using UnityEngine;
using UnityEngine.Windows;

public class XRShipMovement : MonoBehaviour
{
    [Header("Ship Settings")]
    public float moveSpeed = 20f;
    public float rotationSpeed = 30f;
    public float verticalSpeed = 10f;

    private ShipControls controls;

    [Header("Movement Control")]
    public bool isMovementLocked = false;

    void Awake()
    {
        controls = new ShipControls();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        // On update gather player input if movement is locked only get rotation input.

        // Set up variables.
        Vector2 moveInput = new Vector2(0, 0);
        float upInput = 0;
        float downInput = 0;
        Vector2 turnInput = controls.Ship.Turn.ReadValue<Vector2>();

        // If movement is locked do not get player movement inputs (left stick).
        if (!isMovementLocked)
        {
            moveInput = controls.Ship.Move.ReadValue<Vector2>();
            upInput = controls.Ship.Up.ReadValue<float>();
            downInput = controls.Ship.Down.ReadValue<float>();
        }

        HandleMovement(moveInput, turnInput, upInput, downInput); // Call function to apply player input.
    }

    void HandleMovement(Vector2 moveInput, Vector2 turnInput, float upInput, float downInput)
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);

        float pitch = -turnInput.y * rotationSpeed * Time.deltaTime;
        float yaw = turnInput.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(pitch, yaw, 0f, Space.Self);

        if (upInput > 0.1f)
        {
            transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime, Space.World);
        }

        if (downInput > 0.1f)
        {
            transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime, Space.World);
        }

        GetComponent<XRShipHealth>().UpdateFuel(moveDirection.magnitude * moveSpeed * Time.deltaTime); // updateds how much fuel was used

        Debug.Log("Fuel Used: " + moveDirection.magnitude * moveSpeed * Time.deltaTime); // DEBUG: tells how much fuel was used
    }

    // Why is a setter needed for a public variable?
    public void SetMovementLocked(bool locked)
    {
        isMovementLocked = locked;
    }
}
