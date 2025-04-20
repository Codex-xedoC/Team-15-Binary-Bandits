using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerCamera;  // The camera or the player's head position.
    public Vector3 offset;         // The position offset of the UI panels relative to the player.
    private bool isTeleporting = false;

    void Update()
    {
        if (playerCamera != null)
        {
            // Update position of the UI panels relative to the player
            transform.position = playerCamera.position + offset;

            // Optionally, you can make the UI panels always face the camera.
            // This is especially useful if you want the panels to face the player.
            transform.LookAt(playerCamera);
        }
    }
}
