using UnityEngine;

public class RotateAstroids : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 30, 0); // Degrees per second

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
