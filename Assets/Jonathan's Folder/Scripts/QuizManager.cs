using System.Collections;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


public class QuizManager : MonoBehaviour
{
    public List<Questions> QnA;
    public List<Vector3> coords;
    public GameObject[] options;
    public GameObject[] optionss;
    public int currentQuestion;
    //public bool flag;
    public Text QuestionTxt;
    public Text timerText;
    public GameObject Quizpanel;
    public GameObject GoPanel;
    public GameObject ParentPanel;
    public Text wrongAnswers;
    public int panelNum = 0;
    public Text Time;
    public int wscore = 0;
    int totalQuestions = 0;
    public GameObject XRRig;
    public GameObject MultipleChoice, ImageQuestion, TrueFalse;
    private List<Questions> questions = new List<Questions>();
    private Questions currQuestion;
    public Image imageDisplay;
   
    private void Start()
    {
        imageDisplay.gameObject.SetActive(false);
        LoadQuestions();
        totalQuestions = questions.Count;
        GoPanel.SetActive(false);
        generateQuestion();
    }
    
   public void goBack()
    {
        XRRig.transform.position = new Vector3(0,0.97f,1);
    }
    void GameOver()
    {
        Time = timerText;
        panelNum = 0;
        //wrongAnswers.text = wscore.ToString();
        Quizpanel.SetActive(false);
        GoPanel.SetActive(true);
        GoPanel.transform.GetChild(0).GetComponent<Text>().text = Time.text;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Maze1");
    }
    public void correct()
    {
        //MainMenuHandler.Instance.questionCorrect();
        if (panelNum <= 5) //5
        {
            ParentPanel.transform.position = coords[panelNum];
            panelNum++;
            generateQuestion();

        }
        else
        {
            GameOver();
        }
        
    }

    public void retry()
    {
        goBack();
        timerText.text = "{0:00}";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Maze1");
    }

    public void quit()
    {
        Application.Quit();
    }
    public void wrong()
    {
        wscore += 1;
        generateQuestion();
    }

    void setAnswers()
    {
        
            for (int i = 0; i < options.Length; i++)
            {
                if (currQuestion.Choices[i].IsNotNullOrEmpty())
                {
                    optionss[i].SetActive(true);
                    options[i].GetComponent<AnswerScript>().isCorrect = false;
                    // options[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = QnA[currentQuestion].Answers[i];
                    options[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text
                         = currQuestion.Choices[i];

                    if (currQuestion.CorrectAnswer.Trim().Equals(currQuestion.Choices[i].Trim()))
                    {
                        options[i].GetComponent<AnswerScript>().isCorrect = true;

                    }
                }
                else
                {
                    optionss[i].SetActive(false);
                }
            }
        
    }
    void generateQuestion()
    {
        if (questions.Count > 0)
        {
            currentQuestion = Random.Range(0, questions.Count);
            currQuestion = questions[currentQuestion];
            QuestionTxt.text = currQuestion.QuestionText;
            setAnswers();
            if(currQuestion.QuestionType == "Image Question")
            {
                string imageName = "Q" + currQuestion.QNumber;
                Sprite questionImage = Resources.Load<Sprite>(imageName);
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
            }
           
        }
        else
        {
            GameOver();
            Debug.Log("Out of Questions");
        }
    }
    //Other Peoples

  

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
                Questions q = new Questions
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
    /*
     * 
     *   void DisplayQuestion(Questions q)
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
    Questions GetRandomQuestion()
    {
        if (questions.Count == 0)
        {
            Debug.LogError("No questions loaded!");
            return null;
        }
        return questions[Random.Range(0, questions.Count)];
    }
    public void startGamePressed()
    {
        
        currQuestion = GetRandomQuestion();


        if (currQuestion.QuestionType == "Multiple Choice")
        {
            MultipleChoice.SetActive(true);
            Text headerText = MultipleChoice.transform.Find("Header Text").GetComponent<Text>();
            headerText.text = "Question: " + currQuestion.QuestionText;
            Dropdown dropdown = MultipleChoice.transform.Find("Dropdown").GetComponent<Dropdown>();
            
            dropdown.ClearOptions(); // Clear existing options
            dropdown.AddOptions(new List<string> { currQuestion.Choices[0], currQuestion.Choices[1], currQuestion.Choices[2], currQuestion.Choices[3] });
        }
        else if (currQuestion.QuestionType == "Image Question")
        {
            string imageName = "Q" + currQuestion.QNumber; // Assuming QNumber is an integer or string storing the correct question number
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
            headerText.text = "Question: " + currQuestion.QuestionText;
            Dropdown dropdown = ImageQuestion.transform.Find("Dropdown").GetComponent<Dropdown>();
            dropdown.ClearOptions(); // Clear existing options
            dropdown.AddOptions(new List<string> { currQuestion.Choices[0], currQuestion.Choices[1], currQuestion.Choices[2], currQuestion.Choices[3] });
        }
        else if (currQuestion.QuestionType == "True/False")
        {
            TrueFalse.SetActive(true);
            Text headerText = TrueFalse.transform.Find("Header Text").GetComponent<Text>();
            headerText.text = "Question: " + currQuestion.QuestionText;
        }
        else
        {
            Debug.Log("CRASH!!!");
        }

        DisplayQuestion(currQuestion);
    }
    */
}
