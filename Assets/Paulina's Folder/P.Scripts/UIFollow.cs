using UnityEngine;

public class UIFollow : MonoBehaviour
{

    public Transform player;  // Reference to the player's position (could be the VR camera or player object)
    public Vector3 offset;    // Offset to position the UI panel relative to the player

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not set!");
        }
    }

    private void Update()
    {
        // Update the position of the UI panel to follow the player's position
        if (player != null)
        {
            transform.position = player.position + offset;  // Apply the offset to position the UI panel correctly
        }
    }
}

