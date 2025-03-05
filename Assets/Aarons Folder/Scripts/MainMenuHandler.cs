using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuHandler : MonoBehaviour
{
    TextMeshProUGUI numCorrectT;
    TextMeshProUGUI numWrongT;
    TextMeshProUGUI numStreakT;
    TextMeshProUGUI AccuracyRateT;

    public ScoreManager scoreManager;

    TextMeshProUGUI tenCorrectT;
    TextMeshProUGUI hundredCorrectT;
    TextMeshProUGUI tenStreakT;

    public int numCorrect = 0;
    public int numWrong = 0;
    int numStreak = 0;
    float AvgTimePerQuest = 0;
    int AccuracyRate = 0;

    private static MainMenuHandler instance;

    public static MainMenuHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MainMenuHandler>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Codex Added LoadScene Method
    public void LoadScene(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void questionCorrect()
    {
        numCorrect++;
        numStreak++;
        if (scoreManager != null)
            scoreManager.inputScore = numCorrect;
        AccuracyRate = (numCorrect / (numWrong + numCorrect)) * 100;
    }

    public void questionWrong()
    {
        numWrong++;
        numStreak = 0;
        AccuracyRate = (numCorrect / (numWrong + numCorrect)) * 100;
    }

    void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to prevent memory leaks
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called when a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        // Check if the current scene is "Main Menu"
        if (scene.name == "MainMenu")
        {
            numCorrectT = GameObject.Find("numCorrect").GetComponent<TextMeshProUGUI>();
            numWrongT = GameObject.Find("numWrong").GetComponent<TextMeshProUGUI>();
            numStreakT = GameObject.Find("numStreak").GetComponent<TextMeshProUGUI>();
            AccuracyRateT = GameObject.Find("AccuracyRate").GetComponent<TextMeshProUGUI>();

            tenCorrectT = GameObject.Find("tenCorrect").GetComponent<TextMeshProUGUI>();
            hundredCorrectT = GameObject.Find("hundredCorrect").GetComponent<TextMeshProUGUI>();
            tenStreakT = GameObject.Find("tenStreak").GetComponent<TextMeshProUGUI>();

            UpdateMainMenuText();
        }
    }

    // Method to update the Main Menu text
    private void UpdateMainMenuText()
    {
        numCorrectT.text = "# Correct: " + numCorrect;
        numWrongT.text = "# Wrong: " + numWrong;
        numStreakT.text = "# Streak: " + numStreak;
        AccuracyRateT.text = "Accuracy Rate%: " + AccuracyRate;

        if (numCorrect >= 10)
            tenCorrectT.color = Color.green;
        if (numCorrect >= 100)
            hundredCorrectT.color = Color.green;
        if (numStreak >= 10)
            tenStreakT.color = Color.green;
    }

    // Start is still called once on the first scene load
    void Start()
    {
        // Also call UpdateMainMenuText when the script first initializes
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            numCorrectT = GameObject.Find("numCorrect").GetComponent<TextMeshProUGUI>();
            numWrongT = GameObject.Find("numWrong").GetComponent<TextMeshProUGUI>();
            numStreakT = GameObject.Find("numStreak").GetComponent<TextMeshProUGUI>();
            AccuracyRateT = GameObject.Find("AccuracyRate").GetComponent<TextMeshProUGUI>();

            tenCorrectT = GameObject.Find("tenCorrect").GetComponent<TextMeshProUGUI>();
            hundredCorrectT = GameObject.Find("hundredCorrect").GetComponent<TextMeshProUGUI>();
            tenStreakT = GameObject.Find("tenStreak").GetComponent<TextMeshProUGUI>();
            UpdateMainMenuText();
        }
    }
}
