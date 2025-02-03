using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class QuestionHelperCodex : MonoBehaviour
{
    public GameObject UIEmpty;
    public TextMeshProUGUI QuestionBoxText;
    public GameObject Button1, Button2, Button3;
    public GameObject CorrectUI, WrongUI;  // Correct/Incorrect UI Panels
    public TextMeshProUGUI score;
    public TextMeshProUGUI healthText;

    public GameObject QuestionPanel;  // Reference to the Question Panel
    public GameObject AnswerPanelA, AnswerPanelB, AnswerPanelC;

    int scoreI = 0;
    int health = 100;  // Set initial health value (100)

    private string text;
    private List<string> questionsAndAnswers = new List<string>();
    private int index = 0;

    void Start()
    {
        LoadQuestionsFromFile();
        UpdateHealthUI();
    }

    // Loads all questions and answers from the .txt file into a list
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

    // Gets a random question and its answer
    public (string question, string answer) GetRandomQuestion()
    {
        if (questionsAndAnswers.Count == 0)
        {
            Debug.LogError("No questions loaded!");
            return ("", "");
        }

        int randomIndex = Random.Range(0, questionsAndAnswers.Count);
        string randomQuestionEntry = questionsAndAnswers[randomIndex];

        string[] parts = randomQuestionEntry.Split(new[] { "Answer:" }, System.StringSplitOptions.None);

        if (parts.Length < 2)
        {
            Debug.LogError("Invalid question format!");
            return ("", "");
        }

        string question = parts[0].Trim();
        string answer = parts[1].Trim();

        return (question, answer);
    }

    // Called when answer choice 1 is pressed
    public void answerChoice1Pressed()
    {
        Button1.SetActive(false);
        Button2.SetActive(false);
        Button3.SetActive(false);

        if (text == "A")
        {
            StartCoroutine(CorrectAnswerTimer("1"));
        }
        else
        {
            StartCoroutine(WrongAnswerTimer());
        }
    }

    // Called when answer choice 2 is pressed
    public void answerChoice2Pressed()
    {
        Button1.SetActive(false);
        Button2.SetActive(false);
        Button3.SetActive(false);

        if (text == "B")
        {
            StartCoroutine(CorrectAnswerTimer("2"));
        }
        else
        {
            StartCoroutine(WrongAnswerTimer());
        }
    }

    // Called when answer choice 3 is pressed
    public void answerChoice3Pressed()
    {
        Button1.SetActive(false);
        Button2.SetActive(false);
        Button3.SetActive(false);

        if (text == "C")
        {
            StartCoroutine(CorrectAnswerTimer("3"));
        }
        else
        {
            StartCoroutine(WrongAnswerTimer());
        }
    }

    // Handles the correct answer feedback
    private IEnumerator CorrectAnswerTimer(string choice)
    {
        scoreI++;
        score.text = "Score: " + scoreI;
        CorrectUI.SetActive(true); // Show Correct Panel

        yield return new WaitForSeconds(3f);  // Wait before switching back
        CorrectUI.SetActive(false);  // Hide Correct Panel

        RestartGame();
    }

    // Handles the wrong answer feedback
    private IEnumerator WrongAnswerTimer()
    {
        WrongUI.SetActive(true);  // Show Wrong Panel

        yield return new WaitForSeconds(3f);  // Wait before switching back
        WrongUI.SetActive(false);  // Hide Wrong Panel

        RestartGame();
    }

    // Resets the game UI for the next question
    private void RestartGame()
    {
        Button1.SetActive(true);
        Button2.SetActive(true);
        Button3.SetActive(true);

        UIEmpty.SetActive(false);

        // Reset UI for new question
        (string question, string answer) = GetRandomQuestion();
        text = answer;
        QuestionBoxText.text = question;

        // Hide the Question Panel and Answer Panels when done
        QuestionPanel.SetActive(false);
        AnswerPanelA.SetActive(false);
        AnswerPanelB.SetActive(false);
        AnswerPanelC.SetActive(false);
    }

    // Updates the health UI text
    private void UpdateHealthUI()
    {
        healthText.text = "Health: " + health;
    }

    // Updates health when the player collides with an asteroid
    public void UpdateHealth(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Handle Game Over scenario
            health = 0;
            GameOver();
        }
        UpdateHealthUI();
    }

    // Game over logic
    private void GameOver()
    {
        // You can call a Game Over screen or stop the game
        Debug.Log("Game Over!");
        // Optionally show GameOver UI, disable further interactions, etc.
    }

    // When the player presses Start, the game begins
    public void startGamePressed(GameObject button)
    {
        UIEmpty.transform.position = button.transform.position + new Vector3(0f, 0, 0);
        button.SetActive(false);
        UIEmpty.SetActive(true);

        // Get a new question to start the game
        (string question, string answer) = GetRandomQuestion();
        text = answer;
        QuestionBoxText.text = question;

        // Show the Question Panel
        QuestionPanel.SetActive(true);
        AnswerPanelA.SetActive(true);
        AnswerPanelB.SetActive(true);
        AnswerPanelC.SetActive(true);
    }
}
