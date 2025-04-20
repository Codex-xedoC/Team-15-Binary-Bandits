using UnityEngine;
using TMPro;
using System.Collections;

public class XRShipHealth : MonoBehaviour
{
    public static XRShipHealth Instance;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

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

    [Header("Stats Tracking")]
    private int numCorrect = 0;
    private int numWrong = 0;
    private int streak = 0;

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
        Refuel();

        UpdateHealthUI();
        UpdateFuelUI();
        UpdateScoreUI();

        if (GameOverUI != null)
            GameOverUI.SetActive(false);

        if (DamageTextUI != null)
            DamageTextUI.SetActive(false);
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

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth;
    }

    private void GameOver()
    {
        if (GameOverUI != null)
            GameOverUI.SetActive(true);

        SubmitScoreToLeaderboard();
        StartCoroutine(ReturnToMainMenu());
    }

    IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy Bullet"))
        {
            Destroy(collision.gameObject);
            TakeDamage(5);
        }
    }

    public void UpdateFuel(float fuelUsed)
    {
        currentFuel -= fuelUsed;
        UpdateFuelUI();

        if (currentFuel <= 0)
        {
            GameOver();
        }
    }

    private void UpdateFuelUI()
    {
        if (fuelText != null)
            fuelText.text = "Fuel: " + Mathf.Floor(currentFuel);
    }

    public void Refuel()
    {
        currentFuel = maxFuel;
    }

    public void ResetFuel()
    {
        Refuel();
        UpdateFuelUI();
    }

    public void AddCorrect()
    {
        numCorrect++;
        streak++;
        UpdateScoreUI();
    }

    public void AddWrong()
    {
        numWrong++;
        streak = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + numCorrect;
    }

    public void SubmitScoreToLeaderboard()
    {
        ScoreManager sm = FindObjectOfType<ScoreManager>();
        if (sm != null)
        {
            sm.SubmitScore();
        }
    }

    public void PushStatsToMainMenu()
    {
        GameObject correctGO = GameObject.Find("numCorrect");
        GameObject wrongGO = GameObject.Find("numWrong");
        GameObject streakGO = GameObject.Find("numStreak");
        GameObject accuracyGO = GameObject.Find("AccuracyRate");

        if (correctGO != null)
            correctGO.GetComponent<TextMeshProUGUI>().text = "# Correct: " + numCorrect;
        if (wrongGO != null)
            wrongGO.GetComponent<TextMeshProUGUI>().text = "# Wrong: " + numWrong;
        if (streakGO != null)
            streakGO.GetComponent<TextMeshProUGUI>().text = "# Streak: " + streak;

        if (accuracyGO != null)
        {
            int total = numCorrect + numWrong;
            int accuracy = (total > 0) ? Mathf.RoundToInt((float)numCorrect / total * 100) : 0;
            accuracyGO.GetComponent<TextMeshProUGUI>().text = "Accuracy Rate%: " + accuracy;
        }

        GameObject tenCorrect = GameObject.Find("tenCorrect");
        GameObject hundredCorrect = GameObject.Find("hundredCorrect");
        GameObject tenStreak = GameObject.Find("tenStreak");

        if (tenCorrect != null)
            tenCorrect.GetComponent<TextMeshProUGUI>().color = (numCorrect >= 10) ? Color.green : Color.red;

        if (hundredCorrect != null)
            hundredCorrect.GetComponent<TextMeshProUGUI>().color = (numCorrect >= 100) ? Color.green : Color.red;

        if (tenStreak != null)
            tenStreak.GetComponent<TextMeshProUGUI>().color = (streak >= 10) ? Color.green : Color.red;
    }
}
