using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class AaronsLevelScript : MonoBehaviour
{
    public GameObject StartButton, Correct, Wrong;

    public GameObject fishSpawnPoint;
    public GameObject[] fishSpawns;


    public GameObject MultipleChoice, ImageQuestion, TrueFalse;


    public GameObject Fish1;

    public GameObject shark;

    private bool sharkQuestion = false;

    private List<Question> questions = new List<Question>();
    private Question currentQuestion;

    public Image imageDisplay;

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

    private IEnumerator CorrectAnswerTimer()
    {
        MainMenuHandler.Instance.questionCorrect();

        Fish1.SetActive(true);


        int randomNumber = Random.Range(0, 3); // Upper bound is exclusive, so use 6
        //Instantiate(fishSpawns[randomNumber], fishSpawnPoint.transform.position, fishSpawnPoint.transform.rotation);

        Correct.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(5f);

        Fish1.SetActive(false);

        Correct.SetActive(false);

        //RestartGame();
    }

    private IEnumerator WrongAnswerTimer()
    {
        MainMenuHandler.Instance.questionWrong();
        Wrong.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(5f);

        Wrong.SetActive(false);

        //RestartGame();
    }

    public void SubmitAnswer()
    {
        if (currentQuestion.QuestionType == "Multiple Choice")
        {
            Text answerSubmitted = MultipleChoice.transform.Find("Dropdown").transform.Find("Label").GetComponent<Text>();
            if (answerSubmitted.text == currentQuestion.CorrectAnswer)
            {
                // Correct
                StartCoroutine(CorrectAnswerTimer());
            }
            else
            {
                // Wrong
                StartCoroutine(WrongAnswerTimer());
            }
        }
        else if (currentQuestion.QuestionType == "Image Question")
        {
            Text answerSubmitted = MultipleChoice.transform.Find("Dropdown").transform.Find("Label").GetComponent<Text>();
            if (answerSubmitted.text == currentQuestion.CorrectAnswer)
            {
                // Correct
                StartCoroutine(CorrectAnswerTimer());
            }
            else
            {
                // Wrong
                StartCoroutine(WrongAnswerTimer());
            }
        }
        else if (currentQuestion.QuestionType == "True/False")
        {
            Text answerSubmitted = MultipleChoice.transform.Find("Dropdown").transform.Find("Label").GetComponent<Text>();
            if (answerSubmitted.text == currentQuestion.CorrectAnswer)
            {
                // Correct
                StartCoroutine(CorrectAnswerTimer());
            }
            else
            {
                // Wrong
                StartCoroutine(WrongAnswerTimer());
            }
        }
    }

    public void startGamePressed()
    {
        StartButton.SetActive(false);

        currentQuestion = GetRandomQuestion();

        if (currentQuestion.QuestionType == "Multiple Choice")
        {
            MultipleChoice.SetActive(true);
            Text headerText = MultipleChoice.transform.Find("Header Text").GetComponent<Text>();
            headerText.text = "Question: " + currentQuestion.QuestionText;
            Dropdown dropdown = MultipleChoice.transform.Find("Dropdown").GetComponent<Dropdown>();
            dropdown.ClearOptions(); // Clear existing options
            dropdown.AddOptions(new List<string> { currentQuestion.Choices[0], currentQuestion.Choices[1], currentQuestion.Choices[2], currentQuestion.Choices[3] });
        }
        else if (currentQuestion.QuestionType == "Image Question")
        {
            string imageName = "Q" + currentQuestion.QuestionText.Split(' ')[0]; // Assumes question number is the first part of the QuestionText (e.g., "Q297")
            Sprite questionImage = Resources.Load<Sprite>(imageName); // Load the image from Resources

            if (questionImage != null)
            {
                imageDisplay.sprite = questionImage; // Set the image on the UI Image component
                imageDisplay.gameObject.SetActive(true); // Ensure the image is visible
            }
            else
            {
                Debug.LogWarning($"Image {imageName} not found in Resources.");
                imageDisplay.gameObject.SetActive(false); // Hide the image object if not found
            }
            ImageQuestion.SetActive(true);
            Text headerText = ImageQuestion.transform.Find("Header Text").GetComponent<Text>();
            headerText.text = "Question: " + currentQuestion.QuestionText;
            Dropdown dropdown = ImageQuestion.transform.Find("Dropdown").GetComponent<Dropdown>();
            dropdown.ClearOptions(); // Clear existing options
            dropdown.AddOptions(new List<string> { currentQuestion.Choices[0], currentQuestion.Choices[1], currentQuestion.Choices[2], currentQuestion.Choices[3] });
        }
        else if (currentQuestion.QuestionType == "True/False")
        {
            TrueFalse.SetActive(true);
            Text headerText = TrueFalse.transform.Find("Header Text").GetComponent<Text>();
            headerText.text = "Question: " + currentQuestion.QuestionText;
        }
        else
        {
            Debug.Log("CRASH!!!");
        }

        //DisplayQuestion(currentQuestion);
    }
}
