using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class AaronsLevelScript : MonoBehaviour
{
    public GameObject StartButton;

    public GameObject fishSpawnPoint;
    public GameObject[] fishSpawns;


    public GameObject MultipleChoice, MultipleResponse, ImageQuestion, TrueFalse;


    public GameObject Fish1, Fish2, Fish3;

    public GameObject shark;

    bool sharkQuestion = false;

    private string text;

    private List<Question> questions = new List<Question>();
    private Question currentQuestion;

    [System.Serializable]
    public class Question
    {
        public string QuestionText;
        public string[] Choices;
        public string CorrectAnswer;
        public string QuestionType;
    }

    void Start()
    {
        LoadQuestions();
        //Question randomQuestion = GetRandomQuestion();
        //DisplayQuestion(randomQuestion);
    }

    void LoadQuestions()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("QuestionBank"); // CSV must be in "Resources" folder
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found in Resources folder!");
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 2; i < lines.Length; i++) // Start from index 2 to skip headers
        {
            string[] fields = lines[i].Split(',');

            if (fields.Length >= 8) // Ensure there are enough fields
            {
                Question q = new Question
                {
                    QuestionText = fields[2], // Question column
                    Choices = new string[] { fields[3], fields[4], fields[5], fields[6] }, // Answer choices
                    CorrectAnswer = fields[7].Trim(), // Correct answer column
                    QuestionType = fields[1] // Question type column
                };

                questions.Add(q);
            }
        }
    }

    void DisplayQuestion(Question q)
    {
        if (q != null)
        {
            Debug.Log($"Question: {q.QuestionText}");
            Debug.Log($"Question Type: {q.QuestionType}"); // Display question type
            for (int i = 0; i < q.Choices.Length; i++)
            {
                Debug.Log($"{i + 1}. {q.Choices[i]}");
            }
            Debug.Log($"Correct Answer: {q.CorrectAnswer}");
        }
    }

    Question GetRandomQuestion()
    {
        if (questions.Count == 0)
        {
            Debug.LogError("No questions loaded!");
            return null;
        }
        return questions[Random.Range(0, questions.Count)];
    }

    public void answerChoice1Pressed()
    {

    }

    public void answerChoice2Pressed()
    {

    }

    public void answerChoice3Pressed()
    {

    }

    private IEnumerator CorrectAnswerTimer(string fishNum)
    {
        MainMenuHandler.Instance.questionCorrect();
        switch (fishNum)
        {
            case "1":
                Fish1.SetActive(true);
            break;
            case "2":
                Fish2.SetActive(true);
                break;
            case "3":
                Fish3.SetActive(true);
                break;
            default:
                break;
        }

        int randomNumber = Random.Range(0, 3); // Upper bound is exclusive, so use 6
        //Instantiate(fishSpawns[randomNumber], fishSpawnPoint.transform.position, fishSpawnPoint.transform.rotation);

        //CorrectUI.SetActive(true);


        // Wait for the specified duration
        yield return new WaitForSeconds(5f);

        switch (fishNum)
        {
            case "1":
                Fish1.SetActive(false);
                break;
            case "2":
                Fish2.SetActive(false);
                break;
            case "3":
                Fish3.SetActive(false);
                break;
            default:
                break;
        }

        //CorrectUI.SetActive(false);

        //RestartGame();
    }

    private IEnumerator WrongAnswerTimer()
    {
        MainMenuHandler.Instance.questionWrong();
        //WrongUI.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(5f);

        //WrongUI.SetActive(false);

        //RestartGame();
    }

    public void startGamePressed()
    {
        StartButton.SetActive(false);

        currentQuestion = GetRandomQuestion();

        if (currentQuestion.QuestionType == "Multiple Choice")
        {
            MultipleChoice.SetActive(true);
        }
        else if (currentQuestion.QuestionType == "Image Question")
        {
            ImageQuestion.SetActive(true);
        }
        else if (currentQuestion.QuestionType == "Multiple Response")
        {
            MultipleResponse.SetActive(true);
        }
        else if (currentQuestion.QuestionType == "True/False")
        {
            TrueFalse.SetActive(true);
        }
        else
        {
            Debug.Log("CRASH!!!");
        }

        DisplayQuestion(currentQuestion);
    }
}
