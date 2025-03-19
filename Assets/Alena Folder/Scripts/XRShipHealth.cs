namespace AlenaScripts
{
    using UnityEngine;
    using TMPro;

    public class XRShipHealth : MonoBehaviour
    {
        public static XRShipHealth Instance;

        [Header("Health Settings")]
        public int maxHealth = 100;
        public int currentHealth;

        [Header("Score Settings")]
        public int score = 0;

        [Header("UI Elements")]
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI scoreText;
        public GameObject GameOverUI;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            currentHealth = maxHealth;
            UpdateHealthUI();
            UpdateScoreUI();

            if (GameOverUI != null)
            {
                GameOverUI.SetActive(false);
            }
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

        public void AddScore(int points)
        {
            score += points;
            UpdateScoreUI();
        }

        private void UpdateHealthUI()
        {
            if (healthText != null)
            {
                healthText.text = "Health: " + currentHealth;
            }
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null)
            {
                scoreText.text = "Score: " + score;
            }
        }

        private void GameOver()
        {
            if (GameOverUI != null)
            {
                GameOverUI.SetActive(true);
            }
        }
    }
}
