using UnityEngine;

public class XRShipMovement : MonoBehaviour
{
    [Header("Ship Settings")]
    public float moveSpeed = 20f;
    public float rotationSpeed = 30f;
    public float verticalSpeed = 10f;

    private ShipControls controls;

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
        Vector2 moveInput = controls.Ship.Move.ReadValue<Vector2>();
        Vector2 turnInput = controls.Ship.Turn.ReadValue<Vector2>();
        float upInput = controls.Ship.Up.ReadValue<float>();
        float downInput = controls.Ship.Down.ReadValue<float>();

        HandleMovement(moveInput, turnInput, upInput, downInput);
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
    }
}
