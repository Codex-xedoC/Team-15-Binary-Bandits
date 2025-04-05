using UnityEngine;
using TMPro;
using System.Collections;

public class XRShipHealth : MonoBehaviour
{
    public static XRShipHealth Instance;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Score Settings")]
    public int score = 0;
    public int correctAnswers = 0;
    public int wrongAnswers = 0;

    [Header("Fuel System")]
    public int maxFuel = 100;
    public float currentFuel;

    [Header("UI Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI fuelText;
    public GameObject GameOverUI;
    public GameObject DamageTextUI;
    public TextMeshProUGUI damageTextTMP;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        UpdateScoreUI();

        if (GameOverUI != null)
            GameOverUI.SetActive(false);

        if (DamageTextUI != null)
            DamageTextUI.SetActive(false);

        Refuel(); // Reset current fuel to full
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();
        ShowDamageText(damage);

        if (currentHealth <= 0)
            GameOver();
    }

    public void AddScore(int points)
    {
        score += points;
        correctAnswers++;
        UpdateScoreUI();
    }

    public void AddWrong()
    {
        wrongAnswers++;
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void GameOver()
    {
        if (GameOverUI != null)
            GameOverUI.SetActive(true);

        SubmitFinalScore();
        StartCoroutine(ReturnToMainMenu());
    }

    IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(3f);

        Leaderboard leaderboard = FindObjectOfType<Leaderboard>();
        if (leaderboard != null)
        {
            leaderboard.SetLeaderboardEntry(score);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void SubmitFinalScore()
    {
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.SubmitScore();
        }

        Leaderboard leaderboard = FindObjectOfType<Leaderboard>();
        if (leaderboard != null)
        {
            leaderboard.SetLeaderboardEntry(score);
        }
    }

    public void SubmitScoreExternally()
    {
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.SubmitScore();
        }

        Leaderboard leaderboard = FindObjectOfType<Leaderboard>();
        if (leaderboard != null)
        {
            leaderboard.SetLeaderboardEntry(score);
        }
    }

    public void ShowDamageText(int amount)
    {
        if (DamageTextUI != null && damageTextTMP != null)
        {
            damageTextTMP.text = "-" + amount.ToString() + " Health";
            DamageTextUI.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(HideDamageTextAfterDelay());
        }
    }

    private IEnumerator HideDamageTextAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        if (DamageTextUI != null)
            DamageTextUI.SetActive(false);
    }

    public float GetAccuracy()
    {
        int total = correctAnswers + wrongAnswers;
        if (total == 0) return 0;
        return ((float)correctAnswers / total) * 100f;
    }

    // When something colides witht he ship
    private void OnCollisionEnter(Collision collision)
    {
        // If its an enemy bullet destroy bullet and deal damage
        if (collision.gameObject.tag == "Enemy Bullet")
        {
            Destroy(collision.gameObject);
            TakeDamage(5);
        }
    }

    // Reduce fuel level
    public void UpdateFuel(float fuelUsed)
    {
        currentFuel -= fuelUsed;
        UpdateFuelUI();

        if (currentFuel <= 0)
        {
            GameOver();
        }
        
    }

    // Updates fuel UI
    private void UpdateFuelUI()
    {
        if (fuelText != null)
            fuelText.text = "Fuel: " + Mathf.Floor(currentFuel);
    }

    // Reset current fule too full
    public void Refuel()
    {
        currentFuel = maxFuel;
    }
}
