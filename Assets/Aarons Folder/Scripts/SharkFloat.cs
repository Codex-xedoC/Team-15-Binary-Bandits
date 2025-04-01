using UnityEngine;

public class SharkFloat : MonoBehaviour
{
    [SerializeField] private float bobAmount = 1f; // How much it moves up/down
    [SerializeField] private float bobSpeed = 2f;  // Speed of movement

    private float startY; // The original global Y position

    void Start()
    {
        startY = transform.position.y; // Store the initial Y position
    }

    void Update()
    {
        float newY = startY + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
