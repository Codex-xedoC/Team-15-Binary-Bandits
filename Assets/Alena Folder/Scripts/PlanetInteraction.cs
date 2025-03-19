using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlanetInteraction : MonoBehaviour
{
    public GameObject questionPanel;
    public Text questionText;
    public GameObject player;
    public InputActionReference planetInteractAction;  // Separate Action for Planets (A Button)
    public Transform panelSpawnPoint;

    public GameObject correctPanel;
    public GameObject wrongPanel;
    public AudioSource planetFoundAudio;

    private bool isPlayerNear = false;
    private bool isQuestionActive = false;
    private bool hasPlayedAudio = false;
    private string correctAnswer = "";

    private List<string> questionsAndAnswers = new List<string>();

    void Start()
    {
        if (questionPanel != null) questionPanel.SetActive(false);
        if (correctPanel != null) correctPanel.SetActive(false);
        if (wrongPanel != null) wrongPanel.SetActive(false);

        if (planetInteractAction == null)
        {
            Debug.LogError("Planet Interact Action not assigned! Assign it in the inspector.");
            return;
        }

        planetInteractAction.action.Enable();
        planetInteractAction.action.performed += ctx => TryOpenQuestionPanel();  // Now listens for A Button

        LoadQuestionsFromFile();
        Debug.Log("PlanetInteraction script initialized on " + gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player entered planet zone: " + gameObject.name);
            isPlayerNear = true;

            if (!hasPlayedAudio && planetFoundAudio != null)
            {
                planetFoundAudio.Play();
                hasPlayedAudio = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player left planet zone: " + gameObject.name);
            isPlayerNear = false;
            hasPlayedAudio = false;
            CloseQuestionPanel();
        }
    }

    public void TryOpenQuestionPanel()
    {
        if (isPlayerNear && !isQuestionActive)
        {
            Debug.Log("A Button Pressed Near: " + gameObject.name);
            isQuestionActive = true;
            OpenQuestionPanel();
        }
    }

    void OpenQuestionPanel()
    {
        if (questionPanel == null)
        {
            Debug.LogError("Question Panel is MISSING! Assign it in the inspector.");
            return;
        }

        Debug.Log("Opening question panel for " + gameObject.name);

        (string question, string answer) = GetRandomQuestion();
        correctAnswer = answer;

        if (panelSpawnPoint != null)
        {
            questionPanel.transform.position = panelSpawnPoint.position;
            questionPanel.transform.rotation = panelSpawnPoint.rotation;
        }

        questionPanel.SetActive(true);
        questionText.text = question;

        Debug.Log("Question Panel should now be visible for " + gameObject.name);
    }

    void CloseQuestionPanel()
    {
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
            isQuestionActive = false;
            Debug.Log("Question panel closed for " + gameObject.name);
        }
    }

    // **Method Restored: Load Questions from File**
    void LoadQuestionsFromFile()
    {
        TextAsset questionFile = Resources.Load<TextAsset>("computer_science_questions");

        if (questionFile == null)
        {
            Debug.LogError("Question file not found! Ensure it is in Assets/Resources.");
            return;
        }

        string[] lines = questionFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        string questionEntry = "";

        foreach (string line in lines)
        {
            if (line.StartsWith("#"))
            {
                if (!string.IsNullOrEmpty(questionEntry))
                {
                    questionsAndAnswers.Add(questionEntry.Trim());
                }

                questionEntry = line.Substring(line.IndexOf(" ") + 1).Trim();
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

    // **Method Restored: Get Random Question**
    public (string question, string answer) GetRandomQuestion()
    {
        if (questionsAndAnswers.Count == 0)
        {
            Debug.LogError("No questions loaded!");
            return ("No questions available!", "");
        }

        int randomIndex = Random.Range(0, questionsAndAnswers.Count);
        string randomQuestionEntry = questionsAndAnswers[randomIndex];

        string[] parts = randomQuestionEntry.Split(new[] { "Answer:" }, System.StringSplitOptions.None);

        if (parts.Length < 2)
        {
            Debug.LogError("Invalid question format!");
            return ("Error loading question.", "");
        }

        string question = parts[0].Trim();
        string answer = parts[1].Trim();

        return (question, answer);
    }

    // **Handles Answer Selection**
    public void AnswerSelected(string playerAnswer)
    {
        if (playerAnswer == correctAnswer)
        {
            Debug.Log("Correct answer selected!");
            correctPanel.SetActive(true);
            wrongPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Wrong answer selected!");
            correctPanel.SetActive(false);
            wrongPanel.SetActive(true);
        }

        StartCoroutine(ClosePanelsAfterDelay());
    }

    IEnumerator ClosePanelsAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        correctPanel.SetActive(false);
        wrongPanel.SetActive(false);
        CloseQuestionPanel();
    }

    void OnDestroy()
    {
        if (planetInteractAction != null)
        {
            planetInteractAction.action.performed -= ctx => TryOpenQuestionPanel();
        }
    }
}
