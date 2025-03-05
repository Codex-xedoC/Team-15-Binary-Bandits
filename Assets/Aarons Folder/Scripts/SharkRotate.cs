using UnityEngine;

public class SharkRotate : MonoBehaviour
{
    public Transform centerPoint; // The point the shark fin will circle around
    public float radius = 5f;     // Distance from the center
    public float speed = 1f;      // Speed of rotation

    private float angle = 0f;     // Angle to determine position

    void Update()
    {
        if (centerPoint == null)
            return;

        // Increase the angle over time
        angle += speed * Time.deltaTime;

        // Calculate new position
        float x = centerPoint.position.x + Mathf.Cos(angle) * radius;
        float z = centerPoint.position.z + Mathf.Sin(angle) * radius;

        // Update position
        transform.position = new Vector3(x, transform.position.y, z);

        // Make the shark fin face the direction it's moving
        Vector3 direction = new Vector3(-Mathf.Sin(angle), 0, Mathf.Cos(angle)); // Perpendicular to motion
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
