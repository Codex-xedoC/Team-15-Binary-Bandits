using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    public TextMeshProUGUI healthText; // Assign this in the Inspector
    public TextMeshProUGUI scoreText;  // Assign this in the Inspector
    private int health = 100; // Total Health
    private int score = 0; // Initial Score

    void Start()
    {
        UpdateHealthUI();
        UpdateScoreUI();
    }

    public void DecreaseHealth(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            // Trigger Game Over (can be a UI popup or scene change)
            Debug.Log("GAME OVER");
        }
        UpdateHealthUI();
    }

    public void IncreaseScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    private void UpdateHealthUI()
    {
        healthText.text = "Health: " + health;
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }
}
