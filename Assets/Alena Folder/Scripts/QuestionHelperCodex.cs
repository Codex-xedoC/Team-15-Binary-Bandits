using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class QuestionHelperCodex : MonoBehaviour
{
    public GameObject UIEmpty;
    public TextMeshProUGUI QuestionBoxText;
    public GameObject Button1, Button2, Button3;
    public GameObject CorrectUI, WrongUI;
    public TextMeshProUGUI score;
    public TextMeshProUGUI healthText;

    public GameObject QuestionPanel;
    public GameObject AnswerPanelA, AnswerPanelB, AnswerPanelC;

    public GameObject environmentObjects;  // Contains planets & asteroids
    public GameObject startButton;  //  The actual Start Button (Text Poke Button)

    private int scoreI = 0;
    private int health = 100;
    private List<string> questionsAndAnswers = new List<string>();
    private int index = 0;
    private bool isGameActive = false;
    private string correctAnswer;

    void Start()
    {
        LoadQuestionsFromFile();
        UpdateHealthUI();
        ResetGame(); // Ensure everything starts hidden

        //  Ensure Start Button is assigned and linked to StartGame()
        if (startButton != null)
        {
            Button btn = startButton.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(StartGame);
            }
            else
            {
                Debug.LogError("Start Button is missing a Button component!");
            }
        }
        else
        {
            Debug.LogError("Start Button not assigned in Inspector!");
        }
    }

    //  Hides everything until game starts
    private void ResetGame()
    {
        if (environmentObjects != null)
            environmentObjects.SetActive(false);

        QuestionPanel.SetActive(false);
        AnswerPanelA.SetActive(false);
        AnswerPanelB.SetActive(false);
        AnswerPanelC.SetActive(false);
        CorrectUI.SetActive(false);
        WrongUI.SetActive(false);
        UIEmpty.SetActive(false);
    }

    //  Called when Start Button is clicked
    public void StartGame()
    {
        if (environmentObjects != null)
            environmentObjects.SetActive(true);  // Show planets & asteroids

        if (startButton != null)
            startButton.SetActive(false);  // Hide the Start Button

        isGameActive = true;
        scoreI = 0;
        health = 100;
        UpdateHealthUI();
        UpdateScoreUI();

        Debug.Log("Game Started! Environment Activated.");
    }

    //  Loads questions and answers from the text file
    private void LoadQuestionsFromFile()
    {
        TextAsset questionFile = Resources.Load<TextAsset>("computer_science_questions");

        if (questionFile == null)
        {
            Debug.LogError("Question file not found!");
            return;
        }

        string[] lines = questionFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        string questionEntry = "";

        foreach (string line in lines)
        {
            if (line.StartsWith("Question"))
            {
                if (!string.IsNullOrEmpty(questionEntry))
                {
                    questionsAndAnswers.Add(questionEntry.Trim());
                }

                int colonIndex = line.IndexOf(":");
                if (colonIndex != -1 && colonIndex + 1 < line.Length)
                {
                    questionEntry = line.Substring(colonIndex + 1).Trim();
                }
                else
                {
                    questionEntry = line;
                }
            }
            else
            {
                questionEntry += "\n" + line;
            }
        }

        if (!string.IsNullOrEmpty(questionEntry))
        {
            questionsAndAnswers.Add(questionEntry.Trim());
        }
    }

    //  Displays the next question when interacting with a planet
    public void ShowQuestionAtPlanet()
    {
        if (!isGameActive) return;

        index++;
        if (index >= questionsAndAnswers.Count)
        {
            index = 0;
        }

        Debug.Log("Displaying Question Index: " + index);

        (string question, string answer) = GetQuestionByIndex();
        correctAnswer = answer;
        QuestionBoxText.text = question;

        QuestionPanel.SetActive(true);
        AnswerPanelA.SetActive(true);
        AnswerPanelB.SetActive(true);
        AnswerPanelC.SetActive(true);
    }

    //  Retrieves a question by index
    private (string question, string answer) GetQuestionByIndex()
    {
        string questionEntry = questionsAndAnswers[index];

        string[] parts = questionEntry.Split(new[] { "Answer:" }, System.StringSplitOptions.None);
        if (parts.Length < 2)
        {
            Debug.LogError("Invalid question format!");
            return ("", "");
        }

        return (parts[0].Trim(), parts[1].Trim());
    }

    //  Hides the question when the player flies away from a planet
    public void HideQuestion()
    {
        QuestionPanel.SetActive(false);
        AnswerPanelA.SetActive(false);
        AnswerPanelB.SetActive(false);
        AnswerPanelC.SetActive(false);
    }

    //  Called when the player answers a question
    public void AnswerChoicePressed(string choice)
    {
        Button1.SetActive(false);
        Button2.SetActive(false);
        Button3.SetActive(false);

        if (choice == correctAnswer)
        {
            StartCoroutine(CorrectAnswerFeedback());
        }
        else
        {
            StartCoroutine(WrongAnswerFeedback());
        }
    }

    //  Handles correct answer
    private IEnumerator CorrectAnswerFeedback()
    {
        scoreI += 10;
        UpdateScoreUI();
        CorrectUI.SetActive(true);

        yield return new WaitForSeconds(3f);
        CorrectUI.SetActive(false);
        HideQuestion();

        if (scoreI >= 100)
        {
            GameOver(true); // Player wins
        }
    }

    //  Handles incorrect answer
    private IEnumerator WrongAnswerFeedback()
    {
        WrongUI.SetActive(true);

        yield return new WaitForSeconds(3f);
        WrongUI.SetActive(false);
        HideQuestion();
    }

    //  Updates health UI when hitting an asteroid
    public void UpdateHealth(int damage)
    {
        if (!isGameActive) return;

        health -= damage;
        if (health <= 0)
        {
            health = 0;
            GameOver(false); // Player loses
        }
        UpdateHealthUI();
    }

    //  Game over logic (win or lose)
    private void GameOver(bool won)
    {
        isGameActive = false;
        Debug.Log(won ? "Victory! 100 Points Reached!" : "Game Over! Health Depleted!");

        StartCoroutine(ResetAfterDelay());
    }

    //  Resets everything after 10 seconds
    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(10f);
        ResetGame();
        if (startButton != null)
            startButton.SetActive(true);
    }

    //  Updates the UI
    private void UpdateHealthUI()
    {
        healthText.text = "Health: " + health;
    }

    private void UpdateScoreUI()
    {
        score.text = "Score: " + scoreI;
    }
}
