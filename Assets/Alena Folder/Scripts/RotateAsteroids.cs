using UnityEngine;

public class RotateAsteroids : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 30, 0);
    public bool randomizeRotation = true;

    void Start()
    {
        if (randomizeRotation)
        {
            // Assign a random rotation speed per asteroid
            rotationSpeed = new Vector3(
                Random.Range(-50f, 50f),
                Random.Range(-50f, 50f),
                Random.Range(-50f, 50f)
            );
        }
    }

    void Update()
    {
        // Rotate in place instead of orbiting
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }
}
