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
    public GameObject startButton;  // The actual Start Button (Text Poke Button)

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

    public void StartGame()
    {
        if (environmentObjects != null)
            environmentObjects.SetActive(true);

        if (startButton != null)
            startButton.SetActive(false);

        isGameActive = true;
        scoreI = 0;
        health = 100;
        UpdateHealthUI();
        UpdateScoreUI();

        Debug.Log("Game Started! Environment Activated.");
    }

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

    public void ShowQuestionAtPlanet()
    {
        if (!isGameActive) return;

        index = Random.Range(0, questionsAndAnswers.Count);  // ? Pick a random question
        Debug.Log("Displaying Question Index: " + index);

        (string question, string answer) = GetQuestionByIndex();
        correctAnswer = answer;
        QuestionBoxText.text = question;

        QuestionPanel.SetActive(true);
        AnswerPanelA.SetActive(true);
        AnswerPanelB.SetActive(true);
        AnswerPanelC.SetActive(true);

        Debug.Log("Question Displayed: " + question);
    }

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

    public void HideQuestion()
    {
        QuestionPanel.SetActive(false);
        AnswerPanelA.SetActive(false);
        AnswerPanelB.SetActive(false);
        AnswerPanelC.SetActive(false);
    }

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
            GameOver(true);
        }
    }

    private IEnumerator WrongAnswerFeedback()
    {
        WrongUI.SetActive(true);

        yield return new WaitForSeconds(3f);
        WrongUI.SetActive(false);
        HideQuestion();
    }

    public void UpdateHealth(int damage)
    {
        if (!isGameActive) return;

        health -= damage;
        if (health <= 0)
        {
            health = 0;
            GameOver(false);
        }
        UpdateHealthUI();
    }

    private void GameOver(bool won)
    {
        isGameActive = false;
        Debug.Log(won ? "Victory! 100 Points Reached!" : "Game Over! Health Depleted!");

        StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(10f);
        ResetGame();
        if (startButton != null)
            startButton.SetActive(true);
    }

    private void UpdateHealthUI()
    {
        healthText.text = "Health: " + health;
    }

    private void UpdateScoreUI()
    {
        score.text = "Score: " + scoreI;
    }
}
