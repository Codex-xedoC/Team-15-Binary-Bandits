using UnityEngine;
using UnityEngine.UI;

public class PointsCalc : MonoBehaviour
{
    public Text pointsText;  // The UI text to display points (assigned in the Inspector)
    private static int totalPoints = 0;  // Total points are shared across all colliders

    // Points for this collider
    public int pointsValue = 1;  // Default to 1 point, can be set in the Inspector

    // Update the points text display
    private void UpdatePointsText()
    {
        pointsText.text = totalPoints.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is tagged as "fish"
        if (other.CompareTag("fish"))
        {
            // Add points based on the collider that the fish entered
            totalPoints += pointsValue;

            // Update the UI text
            UpdatePointsText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the exiting object is tagged as "fish"
        if (other.CompareTag("fish"))
        {
            // Subtract points based on the collider the fish exited
            totalPoints -= pointsValue;

            // Update the UI text
            UpdatePointsText();
        }
    }
}
