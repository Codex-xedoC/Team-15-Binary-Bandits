using UnityEngine;
using TMPro;

public class XRShipHealth : MonoBehaviour
{
    public static XRShipHealth Instance;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Elements")]
    public TextMeshProUGUI healthText;
    public GameObject GameOverUI;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    private void UpdateHealthUI()
    {
        healthText.text = "Health: " + currentHealth;
    }

    private void GameOver()
    {
        GameOverUI.SetActive(true);
        Debug.Log("Game Over! Ship Destroyed.");
    }
}
