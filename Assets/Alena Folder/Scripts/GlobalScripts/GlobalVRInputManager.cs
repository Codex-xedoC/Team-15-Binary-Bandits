using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GlobalVRInputManager : MonoBehaviour
{
    private GlobalVRControls controls;
    public LayerMask interactableLayer; // Assign "Interactable" layer in Unity

    private void Awake()
    {
        controls = new GlobalVRControls();
        controls.Enable();
    }

    void Update()
    {
        if (controls.Player.Interact.WasPressedThisFrame())
        {
            RaycastHit hit;
            Transform cameraTransform = Camera.main.transform;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 3f, interactableLayer))
            {
                Debug.Log($"Interacted with: {hit.collider.gameObject.name}");
                hit.collider.gameObject.SendMessage("OnInteract", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
