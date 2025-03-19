using UnityEngine;
using UnityEngine.InputSystem;

public class XRShipMovement : MonoBehaviour
{
    public Transform shipTransform;
    public InputActionReference moveAction;
    public InputActionReference rotateAction;
    public InputActionReference verticalUpAction;
    public InputActionReference verticalDownAction;

    public float moveSpeed = 20f;
    public float rotationSpeed = 30f;
    public float verticalSpeed = 10f;

    private Vector2 moveInput;
    private Vector2 rotateInput;
    private float verticalUpInput;
    private float verticalDownInput;

    void Start()
    {
        if (moveAction == null || rotateAction == null || verticalUpAction == null || verticalDownAction == null)
        {
            Debug.LogError("XRShipMovement: Missing input actions.");
            return;
        }

        moveAction.action.Enable();
        rotateAction.action.Enable();
        verticalUpAction.action.Enable();
        verticalDownAction.action.Enable();
    }

    void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        rotateInput = rotateAction.action.ReadValue<Vector2>();
        verticalUpInput = verticalUpAction.action.ReadValue<float>();
        verticalDownInput = verticalDownAction.action.ReadValue<float>();

        MoveShip();
        RotateShip();
        HandleVerticalMovement();
    }

    private void MoveShip()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        shipTransform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);
    }

    private void RotateShip()
    {
        Vector3 rotation = new Vector3(-rotateInput.y * rotationSpeed, rotateInput.x * rotationSpeed, 0);
        shipTransform.Rotate(rotation * Time.deltaTime, Space.Self);
    }

    private void HandleVerticalMovement()
    {
        if (verticalUpInput > 0.1f)
        {
            shipTransform.Translate(Vector3.up * verticalSpeed * Time.deltaTime, Space.World);
        }
        if (verticalDownInput > 0.1f)
        {
            shipTransform.Translate(Vector3.down * verticalSpeed * Time.deltaTime, Space.World);
        }
    }
}
