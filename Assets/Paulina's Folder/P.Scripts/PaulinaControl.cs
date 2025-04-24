using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PaulinaControl : MonoBehaviour
{
    public GameObject UIEmpty, StartUpUI;
    public GameObject ErrorUI, QuestionPUI;
    public GameObject timesUpUI, timerUI;
    public GameObject ImageButtonUI, NumberCorrectUI;
    public GameObject MultipleChoice, ImageQuestion, TrueFalse;

    private List<Question> questions = new List<Question>();
    private Question currentQuestion;

    public Image imageDisplay, imageDisplay2;

    public TextMeshProUGUI QuestionUI, multipleC1, multipleC2, multipleC3, multipleC4;
    public TextMeshProUGUI imageC1, imageC2, imageC3, imageC4;
    public TextMeshProUGUI choice1, choice2;

    [System.Serializable]
    public class Question
    {
        public string QNumber;
        public string QuestionText;
        public string[] Choices;
        public string CorrectAnswer;
        public string QuestionType;
    }

    public float gameDuration = 180f; // 3 minutes in seconds
    private float timeRemaining;
    private bool timerStarted = false;

    public TextMeshProUGUI timerText; // Assign your UI Text here in the Inspector
    public GameObject gameOverScreen; // Assign your Game Over panel here

    private bool gameEnded = false;

    void Start()
    {
 
        //LoadQuestionsFromFile();

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

    public void startGamePressed()
    {
        StartUpUI.SetActive(false);
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
            Debug.Log($"Choices count: {q.Choices.Length}");
            Debug.Log($"Correct Answer: {q.CorrectAnswer}");
        }
    }
    
    private int numQuests = 0;

    public void teleportQuest()
    {   
        UIEmpty.SetActive(true);

        currentQuestion = GetRandomQuestion();

        // Start timer only if it hasn't started yet
        if (!timerStarted)
        {
            NumberCorrectUI.SetActive(true);
            
            timerStarted = true;

            timerUI.SetActive(true);
            timeRemaining = gameDuration;
            gameOverScreen.SetActive(false); // Make sure it's hidden at start
            StartCoroutine(StartCountdown());
        }

        if (currentQuestion.QuestionType == "Multiple Choice")
        {
            ImageQuestion.SetActive(false);
            TrueFalse.SetActive(false);
            QuestionPUI.SetActive(true);
            MultipleChoice.SetActive(true);

            QuestionUI.text = "Question: " + currentQuestion.QuestionText;

            /*
            Text headerText = MultipleChoice.transform.Find("Header Text").GetComponent<Text>();
            headerText.text = "Question: " + currentQuestion.QuestionText;
            
            Dropdown dropdown = MultipleChoice.transform.Find("Dropdown").GetComponent<Dropdown>();
            dropdown.ClearOptions(); // Clear existing options
            dropdown.AddOptions(new List<string> { currentQuestion.Choices[0], currentQuestion.Choices[1], currentQuestion.Choices[2], currentQuestion.Choices[3] });
            */

            numQuests++;
            SetMultipleChoiceAnswers();

        }
        else if (currentQuestion.QuestionType == "Image Question")
        {
            string imageName = "Q" + currentQuestion.QNumber.Trim(); // Assuming QNumber is an integer or string storing the correct question number
            //Sprite questionImage = Resources.Load<Sprite>(imageName); // Load the image from Resou
            
            //string imageName = currentQuestion.ImageName.Trim();
            Debug.Log($"[Image Question] Trying to load image: Resources/Images/{imageName}");

            Sprite questionImage = Resources.Load<Sprite>(imageName);
            if (questionImage != null)
            {
                imageDisplay.sprite = questionImage;
                imageDisplay.gameObject.SetActive(true);
                Debug.Log("[Image Question] Image loaded and assigned successfully.");

                imageDisplay2.sprite = questionImage;
            }
            else
            {
                Debug.LogWarning($"[Image Question] Image NOT found: Resources/Images/{imageName}. Make sure the image is in Assets/Resources/Images and has no extension in the name.");
                imageDisplay.gameObject.SetActive(false);
            }

            MultipleChoice.SetActive(false);
            TrueFalse.SetActive(false);
            ImageButtonUI.SetActive(true);
            QuestionPUI.SetActive(true);
            ImageQuestion.SetActive(true);

            QuestionUI.text = "Question: " + currentQuestion.QuestionText;

            /*
            Text headerText = ImageQuestion.transform.Find("Header Text").GetComponent<Text>();
            headerText.text = "Question: " + currentQuestion.QuestionText;
            
            Dropdown dropdown = ImageQuestion.transform.Find("Dropdown").GetComponent<Dropdown>();
            dropdown.ClearOptions(); // Clear existing options
            dropdown.AddOptions(new List<string> { currentQuestion.Choices[0], currentQuestion.Choices[1], currentQuestion.Choices[2], currentQuestion.Choices[3] });
            */

            numQuests++;
            SetMultipleChoiceAnswers();
        }
        else if (currentQuestion.QuestionType == "True/False")
        {
            MultipleChoice.SetActive(false);
            ImageQuestion.SetActive(false);
            QuestionPUI.SetActive(true);
            TrueFalse.SetActive(true);

            QuestionUI.text = "Question: " + currentQuestion.QuestionText;

            /*
            Text headerText = TrueFalse.transform.Find("Header Text").GetComponent<Text>();
            headerText.text = "Question: " + currentQuestion.QuestionText;
            */

            numQuests++;

            if (currentQuestion.Choices.Length >= 2)
            {
                choice1.text = currentQuestion.Choices[0];
                choice2.text = currentQuestion.Choices[1];
            }
            else
            {
                Debug.LogError("True/False question is missing choices!");
            }
        }
        else
        {
            Debug.Log("CRASH!!!");
        }

        DisplayQuestion(currentQuestion);
    }

    public void SetMultipleChoiceAnswers()
    {
        multipleC1.text = currentQuestion.Choices[0];
        multipleC2.text = currentQuestion.Choices[1];
        multipleC3.text = currentQuestion.Choices[2];
        multipleC4.text = currentQuestion.Choices[3];

        imageC1.text = currentQuestion.Choices[0];
        imageC2.text = currentQuestion.Choices[1];
        imageC3.text = currentQuestion.Choices[2];
        imageC4.text = currentQuestion.Choices[3];
    }

    public void SubmitAnswer(TextMeshProUGUI choiceMade)
    {
        if (currentQuestion.QuestionType == "Multiple Choice")
        {
            //Text answerSubmitted = MultipleChoice.transform.Find("Dropdown").transform.Find("Label").GetComponent<Text>();
            //Text answerSubmitted = choiceMade.text;
            
            if (choiceMade.text.ToLower() == currentQuestion.CorrectAnswer.ToLower())
            {
                Debug.Log("Going into CorrectAnswerTimer");
                // Correct
                StartCoroutine(CorrectAnswerTimer(choiceMade));
            }
            else
            {
                Debug.Log("Going into WrongAnswerTimer");
                // Wrong
                StartCoroutine(WrongAnswerTimer(choiceMade));
                
            }
        }
        else if (currentQuestion.QuestionType == "Image Question")
        {
            //Text answerSubmitted = MultipleChoice.transform.Find("Dropdown").transform.Find("Label").GetComponent<Text>();
            //Text answerSubmitted = choiceMade;
            if (choiceMade.text.ToLower() == currentQuestion.CorrectAnswer.ToLower())
            {
                // Correct
                StartCoroutine(CorrectAnswerTimer(choiceMade));
            }
            else
            {
                // Wrong
                StartCoroutine(WrongAnswerTimer(choiceMade));
            }
        }
        else if (currentQuestion.QuestionType == "True/False")
        {
            //Text answerSubmitted = choiceMade;
            //Text answerSubmitted = TrueFalse.transform.Find("Dropdown").transform.Find("Label").GetComponent<Text>();
            if (choiceMade.text.ToLower() == currentQuestion.CorrectAnswer.ToLower())
            {
                // Correct
                StartCoroutine(CorrectAnswerTimer(choiceMade));
            }
            else
            {
                // Wrong
                StartCoroutine(WrongAnswerTimer(choiceMade));
            }
        }
    }

    public void TurnOffUI()
    {
        ImageButtonUI.SetActive(false);
        QuestionPUI.SetActive(false);
        ImageQuestion.SetActive(false);
        MultipleChoice.SetActive(false);
        TrueFalse.SetActive(false);
    }

    public TextMeshProUGUI numCorrectText, numCorrectFinal, numPercent; // Assign this in Unity Inspector
    private int numCorrect = 0; // Counter for correct answers

    private IEnumerator CorrectAnswerTimer(TextMeshProUGUI choiceMade)
    {
        Debug.Log("Starting CorrectAnswerTimer");
        MainMenuHandler.Instance.questionCorrect();

        // Go up from choiceMade to button front
        Transform buttonFront = choiceMade.transform.parent;
        // Find correctUI and wrongUI under button front
        Transform correctUI = buttonFront.Find("CorrectSprite");

        correctUI.gameObject.SetActive(true);

        numCorrect++; // Increment the correct answers count
        numCorrectText.text = numCorrect.ToString(); // Update the UI

        // Wait for the specified duration
        yield return new WaitForSeconds(5f);

        correctUI.gameObject.SetActive(false);

        TurnOffUI();
        UIEmpty.SetActive(false);
    }

    private IEnumerator WrongAnswerTimer(TextMeshProUGUI choiceMade)
    {
        Debug.Log("Starting WrongAnswerTimer");
        MainMenuHandler.Instance.questionWrong();

        Transform buttonFront = choiceMade.transform.parent;
        Transform wrongUI = buttonFront.Find("WrongSprite");

        wrongUI.gameObject.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(5f);

        wrongUI.gameObject.SetActive(false);
    }

    private IEnumerator ErrorTimer()
    {
        ErrorUI.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(5f);

        ErrorUI.SetActive(false);
    }

    private IEnumerator StartCountdown()
    {
        while (timeRemaining > 0 && !gameEnded)
        {
            UpdateTimerUI();
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
        }

        if (!gameEnded)
        {
            GameOver();
        }
    }

    private IEnumerator TimesUp() 
    {
        timesUpUI.SetActive(true);
        // Wait for the specified duration
        yield return new WaitForSeconds(5f);
        timesUpUI.SetActive(false);
    }

    public void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GameOver()
    {
        gameEnded = true;
        Debug.Log("Time's up!");
        timerUI.SetActive(false);
        NumberCorrectUI.SetActive(false);
        UIEmpty.SetActive(false);

        StartCoroutine(TimesUp());

        DisplayPercentAccuracy();

        gameOverScreen.SetActive(true);
        // disable gameplay or stop player input here
        //SceneManager.ReturnMainMenu();
    }

    // Optional: Call this method to stop timer early if player finishes early
    public void EndGameEarly()
    {
        if (!gameEnded)
        {
            GameOver();
        }
    }

    public void DisplayPercentAccuracy()
    {
        float numPercentAcc = ((float)numCorrect / numQuests) * 100;
        numCorrectFinal.text = "Number Correct: " + numCorrect.ToString(); // Update the UI
        numPercent.text = "Percent Accuracy: " + numPercentAcc.ToString("F2") + "% ";

    }

}
