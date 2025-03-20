using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlanetInteraction : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject questionPanel;
    public Text questionText;
    public GameObject player;
    public Transform panelSpawnPoint;

    [Header("Input Actions")]
    public InputActionReference planetInteractAction;
    public InputActionReference rightTriggerAction;

    [Header("Answer Panels")]
    public GameObject correctPanel;
    public GameObject wrongPanel;

    [Header("Audio")]
    public AudioSource planetFoundAudio;

    private bool isPlayerNear = false;
    private bool isQuestionActive = false;
    private bool hasPlayedAudio = false;
    private string correctAnswer = "";

    private GameObject currentPlanet = null;
    private List<string> questionsAndAnswers = new List<string>();

    void Start()
    {
        if (questionPanel != null) questionPanel.SetActive(false);
        if (correctPanel != null) correctPanel.SetActive(false);
        if (wrongPanel != null) wrongPanel.SetActive(false);

        if (planetInteractAction == null || rightTriggerAction == null)
        {
            Debug.LogError("PlanetInteraction: Missing input actions. Assign them in the Inspector.");
            return;
        }

        planetInteractAction.action.Enable();
        planetInteractAction.action.performed += ctx => TryOpenQuestionPanel();

        rightTriggerAction.action.Enable();
        rightTriggerAction.action.performed += ctx => TryOpenQuestionPanel();

        LoadQuestionsFromFile();
    }

    void Update()
    {
        DetectPlanetInView();
    }

    void DetectPlanetInView()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 15f))
        {
            if (hit.collider.CompareTag("Planet"))
            {
                currentPlanet = hit.collider.gameObject;
                return;
            }
        }
        currentPlanet = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log($"Player entered planet zone: {gameObject.name}");
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
            Debug.Log($"Player left planet zone: {gameObject.name}");
            isPlayerNear = false;
            hasPlayedAudio = false;
            CloseQuestionPanel();
        }
    }

    public void TryOpenQuestionPanel()
    {
        Debug.Log($"[PlanetInteraction] Attempting to open question panel for {gameObject.name}");

        if ((isPlayerNear || currentPlanet != null) && !isQuestionActive)
        {
            Debug.Log($"[PlanetInteraction] Question panel should now open for {gameObject.name}");
            isQuestionActive = true;
            OpenQuestionPanel();
        }
        else
        {
            Debug.Log("[PlanetInteraction] Interaction ignored: Either not near a planet or already active.");
        }
    }

    void OpenQuestionPanel()
    {
        if (questionPanel == null)
        {
            Debug.LogError("[ERROR] Question Panel is missing! Assign it in the Inspector.");
            return;
        }

        Debug.Log($"[PlanetInteraction] Opening question panel for {gameObject.name}");

        (string question, string answer) = GetRandomQuestion();
        correctAnswer = answer;

        // Ensure panel is active
        questionPanel.SetActive(true);

        // Reset panel visibility in case it's disabled by Canvas settings
        CanvasGroup canvasGroup = questionPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        // Position the panel in front of the player
        Transform playerCamera = Camera.main.transform;
        if (playerCamera != null)
        {
            questionPanel.transform.SetParent(null);
            questionPanel.transform.position = playerCamera.position + playerCamera.forward * 1.5f;
            questionPanel.transform.rotation = Quaternion.LookRotation(questionPanel.transform.position - playerCamera.position);
        }
        else
        {
            Debug.LogError("[ERROR] Player Camera not found!");
            return;
        }

        questionText.text = question;

        Debug.Log("[PlanetInteraction] Question Panel should now be visible.");
    }

    void CloseQuestionPanel()
    {
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
            isQuestionActive = false;
            Debug.Log($"Question panel closed for {gameObject.name}");
        }
    }

    void LoadQuestionsFromFile()
    {
        TextAsset questionFile = Resources.Load<TextAsset>("computer_science_questions");

        if (questionFile == null)
        {
            Debug.LogError("Question file not found. Ensure it is in Assets/Resources.");
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

    public (string question, string answer) GetRandomQuestion()
    {
        if (questionsAndAnswers.Count == 0)
        {
            Debug.LogError("No questions loaded.");
            return ("No questions available.", "");
        }

        int randomIndex = Random.Range(0, questionsAndAnswers.Count);
        string randomQuestionEntry = questionsAndAnswers[randomIndex];

        string[] parts = randomQuestionEntry.Split(new[] { "Answer:" }, System.StringSplitOptions.None);

        if (parts.Length < 2)
        {
            Debug.LogError("Invalid question format.");
            return ("Error loading question.", "");
        }

        return (parts[0].Trim(), parts[1].Trim());
    }

    public void AnswerSelected(string playerAnswer)
    {
        if (playerAnswer == correctAnswer)
        {
            Debug.Log("Correct answer selected.");
            correctPanel.SetActive(true);
            wrongPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Wrong answer selected.");
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

        if (rightTriggerAction != null)
        {
            rightTriggerAction.action.performed -= ctx => TryOpenQuestionPanel();
        }
    }
}