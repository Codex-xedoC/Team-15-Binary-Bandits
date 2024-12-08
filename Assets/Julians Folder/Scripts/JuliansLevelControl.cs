using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class JuliansLevelControl : MonoBehaviour
{
    public GameObject UIEmpty;
    public TextMeshProUGUI QuestionBoxText;
    public GameObject StartButton;

    public GameObject Button1, Button2, Button3;

    public GameObject CorrectUI, WrongUI;

    public GameObject[] grabables;

    public GameObject player;

    private string text;

    private List<string> questionsAndAnswers = new List<string>();

    private int index = 0;

    void Start()
    {
        LoadQuestionsFromFile();
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

        return (question, answer);
    }





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

    private IEnumerator CorrectAnswerTimer(string fishNum)
    {
        MainMenuHandler.Instance.questionCorrect();



        CorrectUI.SetActive(true);


        // Wait for the specified duration
        yield return new WaitForSeconds(3f);



        CorrectUI.SetActive(false);

        RestartGame();
    }

    private IEnumerator WrongAnswerTimer()
    {
        MainMenuHandler.Instance.questionWrong();
        WrongUI.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(3f);

        WrongUI.SetActive(false);

        RestartGame();
    }

    private void RestartGame()
    {
        Button1.SetActive(true);
        Button2.SetActive(true);
        Button3.SetActive(true);

        //player.transform.position = grabables[1 + index].transform.position + new Vector3(0, -1, -1.5f);
        //UIEmpty.transform.position = grabables[1 + index].transform.position + new Vector3(0.3f, 0.5f, 0);
        //index++;

        (string question, string answer) = GetRandomQuestion();
        text = answer;
        QuestionBoxText.text = question;


    }

    public void startGamePressed()
    {
        StartButton.SetActive(false);
        UIEmpty.SetActive(true);

        (string question, string answer) = GetRandomQuestion();
        text = answer;
        QuestionBoxText.text = question;
    }
}
