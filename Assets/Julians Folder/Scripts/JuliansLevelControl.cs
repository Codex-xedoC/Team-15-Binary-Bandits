using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class JuliansLevelControl : MonoBehaviour
{
    /*
    
    public TextMeshProUGUI QuestionBoxText;
    public GameObject StartButton;

    public GameObject Button1, Button2, Button3;

    public GameObject CorrectUI, WrongUI;



    public GameObject EndButton; // Button to restart or return to menu
    private int questionLimit = 16; // Max number of questions
    private int questionCount = 0;  // Tracks how many questions have been asked

    private string text;

    private List<string> questionsAndAnswers = new List<string>();

    
    */
    public GameObject UIEmpty;
    public GameObject[] grabables;

    public GameObject player;
    private int index = 0;
    public GameObject StartButton, Correct, Wrong;
    public GameObject HowToPlay;

    //public GameObject fishSpawnPoint;
    //public GameObject[] fishSpawns;


    public GameObject MultipleChoice, ImageQuestion, TrueFalse;


    //public GameObject Fish1;

    //public GameObject shark, sharkReal;

    //private bool sharkQuestion = false;

    private List<Question> questions = new List<Question>();
    private Question currentQuestion;

    //private List<GameObject> spawnedFish = new List<GameObject>();

    public Image imageDisplay;

    [System.Serializable]

    public class Question
    {
        public string QNumber;
        public string QuestionText;
        public string[] Choices;
        public string CorrectAnswer;
        public string QuestionType;
    }

    void Start()
    {
        //LoadQuestionsFromFile();
        LoadQuestions();
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

        for (int i = 1; i < lines.Length; i++) // Start from index 1 to skip the header
        {
            string[] fields = lines[i].Split(',');

            if (fields.Length >= 8) // Ensure there are enough fields
            {
                Question q = new Question
                {
                    QNumber = fields[0].Trim(), // Assign the question number from column 1
                    QuestionType = fields[1].Trim(), // Question type column
                    QuestionText = fields[2].Trim(), // Question column
                    Choices = new string[] { fields[3].Trim(), fields[4].Trim(), fields[5].Trim(), fields[6].Trim() }, // Answer choices
                    CorrectAnswer = fields[7].Trim() // Correct answer column
                };

                questions.Add(q);
            }
        }
    }

    /* Loads all questions and answers from the .txt file into a list
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
    */

    /* Gets a random question and its answer
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


    */

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

    /*
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
    */

    private IEnumerator CorrectAnswerTimer()
    {
        MainMenuHandler.Instance.questionCorrect();

        //Fish1.SetActive(true);


        int randomNumber = Random.Range(0, 3); // Upper bound is exclusive, so use 6
        //GameObject newFish = Instantiate(fishSpawns[randomNumber], fishSpawnPoint.transform.position, fishSpawnPoint.transform.rotation);

        //spawnedFish.Add(newFish);

        Correct.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(5f);

        if (index + 1 < grabables.Length)
        {
            player.transform.position = grabables[1 + index].transform.position + new Vector3(0, -1, -1.5f);
            UIEmpty.transform.position = grabables[1 + index].transform.position + new Vector3(0.3f, 0.5f, 0);
            index++;
            //RestartGame();
        }
        else
        {
            Debug.Log("All questions answered. Showing End Button.");
            //EndButton.SetActive(true);
        }

        //Fish1.SetActive(false);

        Correct.SetActive(false);

        RestartGame();
    }

    /*
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
    */

    /*
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
    */

    /*
    private IEnumerator CorrectAnswerTimer(string fishNum)
    {
        MainMenuHandler.Instance.questionCorrect();



        CorrectUI.SetActive(true);


        // Wait for the specified duration
        yield return new WaitForSeconds(3f);



        CorrectUI.SetActive(false);


    }
    */


    private IEnumerator WrongAnswerTimer()
    {
        MainMenuHandler.Instance.questionWrong();
        Wrong.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(3f);

        Wrong.SetActive(false);

        RestartGame();
    }

    private void RestartGame()
    {
        /*
        questionCount++; // Increment question count

        if (questionCount >= questionLimit)
        {
            // Hide answer buttons
            Button1.SetActive(false);
            Button2.SetActive(false);
            Button3.SetActive(false);

            
            
            // Show the end button -- should be true but the menu button is already there => redundant
            EndButton.SetActive(false);
            return; // Stop asking new questions
        }

        Button1.SetActive(true);
        Button2.SetActive(true);
        Button3.SetActive(true);

        (string question, string answer) = GetRandomQuestion();
        text = answer;
        QuestionBoxText.text = question;
        */
        startGamePressed();

    }

    public void SubmitAnswer()
    {
        if (currentQuestion.QuestionType == "Multiple Choice")
        {
            Text answerSubmitted = MultipleChoice.transform.Find("Dropdown").transform.Find("Label").GetComponent<Text>();
            MultipleChoice.SetActive(false);
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
            ImageQuestion.SetActive(false);
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
            TrueFalse.SetActive(false);
            Text answerSubmitted = TrueFalse.transform.Find("Dropdown").transform.Find("Label").GetComponent<Text>();
            if (answerSubmitted.text.ToLower() == currentQuestion.CorrectAnswer.ToLower())
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
        //sharkQuestion = false;
        //shark.SetActive(false);
        StartButton.SetActive(false);
        HowToPlay.SetActive(false);

        currentQuestion = GetRandomQuestion();

        int randomNumber1 = Random.Range(1, 4); // Upper bound is exclusive, so use 6
        Debug.Log(randomNumber1);
        /*
        if (randomNumber1 == 3)
        {
            sharkQuestion = true;
            shark.SetActive(true);
        }
        */

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
            string imageName = "Q" + currentQuestion.QNumber; // Assuming QNumber is an integer or string storing the correct question number
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

        DisplayQuestion(currentQuestion);
    }

    /*
    public void EndGame()
    {
        // Reset question count
        questionCount = 0;
        index = 0;

        // Hide the end button
        EndButton.SetActive(false);

        // Show start button to let player choose next action
        StartButton.SetActive(true);
    }
    */
}
