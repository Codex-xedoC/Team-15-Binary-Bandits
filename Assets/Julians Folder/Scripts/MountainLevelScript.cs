using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class MountainLevelScript : MonoBehaviour
{
    public GameObject UIEmpty;
    public TextMeshProUGUI QuestionBoxText;
    //public XRLocomotionSystem locomotionSystem;

    public int points;
    public int lives;


    public GameObject CorrectUI, WrongUI;

    private string text;

    private List<string> questionsAndAnswers = new List<string>();
    private string currentAnswer = "";
    public bool questionDisplayed = false;
    public bool questionAnswered = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadQuestionsFromFile();
        points = 0;
        lives = 3;
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

                // Remove "Question X:" prefix
                int colonIndex = line.IndexOf(":");
                if (colonIndex != -1 && colonIndex + 1 < line.Length)
                {
                    questionEntry = line.Substring(colonIndex + 1).Trim(); // Start a new question without the prefix
                }
                else
                {
                    questionEntry = line;
                }
            }
            else
            {
                questionEntry += "\n" + line; // Append to the current question block
            }
        }

        // Add the last question entry
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

        // Split the entry into question text and answer
        string[] parts = randomQuestionEntry.Split(new[] { "Answer:" }, System.StringSplitOptions.None);

        if (parts.Length < 2)
        {
            Debug.LogError("Invalid question format!");
            return ("", "");
        }

        string question = parts[0].Trim(); // Question and options
        string answer = parts[1].Trim(); // Correct answer

        QuestionBoxText.text = question;

        UIEmpty.SetActive(true);
        questionDisplayed = true;

        Debug.Log("Question: " + question);
        Debug.Log("Answer: " + answer);

        return (question, answer);
        
        
       
    }
    
    private void HandleAnswer(string playerAnswer)
    {
        if (playerAnswer.Equals(currentAnswer, System.StringComparison.OrdinalIgnoreCase))
        {
            CorrectUI.SetActive(true);
            Debug.Log("Correct Answer!");
            points++;
        }
        else
        {
            WrongUI.SetActive(true);
            Debug.Log("Wrong Answer!");
            lives--;
        }

        questionAnswered = true;
        UIEmpty.SetActive(false); // Hide question UI
        StartCoroutine(ResetUI());
    }

    private IEnumerator ResetUI()
    {
        yield return new WaitForSeconds(2f); // Display feedback for 2 seconds
        CorrectUI.SetActive(false);
        WrongUI.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
