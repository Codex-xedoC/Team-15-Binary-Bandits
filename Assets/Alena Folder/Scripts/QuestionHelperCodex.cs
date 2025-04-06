using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuestionHelperCodex : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject MultipleChoice;
    public GameObject TrueFalse;
    public GameObject ImageQuestion;
    public GameObject CorrectPanel;
    public GameObject WrongPanel;

    [Header("Dropdowns")]
    public Dropdown MultipleChoiceDropdown;
    public Dropdown TrueFalseDropdown;
    public Dropdown ImageQuestionDropdown;

    [Header("Question Texts")]
    public Text MultipleChoiceQuestionText;
    public Text TrueFalseQuestionText;
    public Text ImageQuestionText;

    [Header("Image Display")]
    public Image ImageDisplay;

    [Header("Navigation UI")]
    public GameObject BackPanel;

    [Header("Player")]
    public GameObject player;

    private List<Question> questions = new List<Question>();
    private Question currentQuestion;
    private GameObject lastActivePanel;

    private Vector3 lastQuestionPosition;
    private Quaternion lastQuestionRotation;

    [System.Serializable]
    public class Question
    {
        public string QNumber;
        public string QuestionType;
        public string QuestionText;
        public string[] Choices;
        public string CorrectAnswer;
    }

    void Start()
    {
        LoadQuestions();
    }

    private void LoadQuestions()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("QuestionBank");
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found in Resources folder.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');

            if (fields.Length >= 8)
            {
                Question q = new Question
                {
                    QNumber = fields[0].Trim(),
                    QuestionType = fields[1].Trim(),
                    QuestionText = fields[2].Trim(),
                    Choices = new string[]
                    {
                        fields[3].Trim(),
                        fields[4].Trim(),
                        fields[5].Trim(),
                        fields[6].Trim()
                    },
                    CorrectAnswer = fields[7].Trim()
                };

                questions.Add(q);
            }
        }

        Debug.Log($"Loaded {questions.Count} questions.");
    }

    public void DisplayNewQuestion()
    {
        if (questions.Count == 0)
        {
            Debug.LogError("No questions loaded.");
            return;
        }

        currentQuestion = questions[Random.Range(0, questions.Count)];

        HideAllPanels();

        GameObject activePanel = null;

        switch (currentQuestion.QuestionType)
        {
            case "Multiple Choice":
                MultipleChoice.SetActive(true);
                MultipleChoiceQuestionText.text = "Question: " + currentQuestion.QuestionText;
                MultipleChoiceDropdown.ClearOptions();
                MultipleChoiceDropdown.AddOptions(new List<string>(currentQuestion.Choices));
                activePanel = MultipleChoice;
                break;

            case "True/False":
                TrueFalse.SetActive(true);
                TrueFalseQuestionText.text = "Question: " + currentQuestion.QuestionText;
                TrueFalseDropdown.ClearOptions();
                TrueFalseDropdown.AddOptions(new List<string> { "True", "False" });
                activePanel = TrueFalse;
                break;

            case "Image Question":
                ImageQuestion.SetActive(true);
                ImageQuestionText.text = "Question: " + currentQuestion.QuestionText;

                Sprite loadedImage = Resources.Load<Sprite>("Q" + currentQuestion.QNumber);
                if (loadedImage != null)
                {
                    ImageDisplay.sprite = loadedImage;
                    ImageDisplay.gameObject.SetActive(true);
                }
                else
                {
                    ImageDisplay.gameObject.SetActive(false);
                }

                ImageQuestionDropdown.ClearOptions();
                ImageQuestionDropdown.AddOptions(new List<string>(currentQuestion.Choices));
                activePanel = ImageQuestion;
                break;

            default:
                Debug.LogWarning("Unsupported question type: " + currentQuestion.QuestionType);
                return;
        }

        if (activePanel != null)
        {
            PositionPanel(activePanel);
            lastActivePanel = activePanel;
        }
    }

    private void PositionPanel(GameObject panel)
    {
        Transform cam = Camera.main.transform;
        Vector3 spawnPos = cam.position + cam.forward * 3f;

        panel.transform.SetParent(null);
        panel.transform.position = spawnPos;
        panel.transform.rotation = Quaternion.LookRotation(cam.forward, cam.up);
        panel.transform.localScale = Vector3.one * 0.005f;

        lastQuestionPosition = spawnPos;
        lastQuestionRotation = panel.transform.rotation;

        if (BackPanel != null)
        {
            BackPanel.transform.SetParent(null);
            BackPanel.transform.rotation = panel.transform.rotation;

            Vector3 sideOffset = panel.transform.right * -2.5f;
            BackPanel.transform.position = panel.transform.position + sideOffset;
            BackPanel.transform.localScale = Vector3.one;
            BackPanel.SetActive(true);

            CanvasGroup backGroup = BackPanel.GetComponent<CanvasGroup>();
            if (backGroup != null)
            {
                backGroup.alpha = 1f;
                backGroup.interactable = true;
                backGroup.blocksRaycasts = true;
            }
        }

        CanvasGroup group = panel.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
    }

    private void HideAllPanels()
    {
        MultipleChoice.SetActive(false);
        TrueFalse.SetActive(false);
        ImageQuestion.SetActive(false);
        CorrectPanel.SetActive(false);
        WrongPanel.SetActive(false);

        if (BackPanel != null)
            BackPanel.SetActive(false);
    }

    public void SubmitAnswer()
    {
        string selected = "";

        if (currentQuestion.QuestionType == "Multiple Choice")
        {
            selected = MultipleChoiceDropdown.options[MultipleChoiceDropdown.value].text;
            MultipleChoice.SetActive(false);
        }
        else if (currentQuestion.QuestionType == "True/False")
        {
            selected = TrueFalseDropdown.options[TrueFalseDropdown.value].text;
            TrueFalse.SetActive(false);
        }
        else if (currentQuestion.QuestionType == "Image Question")
        {
            selected = ImageQuestionDropdown.options[ImageQuestionDropdown.value].text;
            ImageQuestion.SetActive(false);
        }

        if (selected == currentQuestion.CorrectAnswer)
        {
            XRGameScore.Instance.AddScore(10);
            ShowPanelImmediate(CorrectPanel);
            MainMenuHandler.Instance.questionCorrect();
        }
        else
        {
            ShowPanelImmediate(WrongPanel);
            MainMenuHandler.Instance.questionWrong();
        }
    }

    private void ShowPanelImmediate(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogError("Result panel is null.");
            return;
        }

        Transform t = panel.transform;
        while (t != null)
        {
            t.gameObject.SetActive(true);
            t = t.parent;
        }

        panel.SetActive(true);

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        panel.transform.position = lastQuestionPosition + Camera.main.transform.forward * 0.5f;
        panel.transform.rotation = lastQuestionRotation;
        panel.transform.localScale = Vector3.one * 0.007f;

        if (panel.GetComponent<BoxCollider>() == null)
        {
            BoxCollider collider = panel.AddComponent<BoxCollider>();
            collider.size = new Vector3(500, 300, 10);
            collider.isTrigger = false;
        }

        StartCoroutine(ResetTargetAcquiredFlag());
    }

    private IEnumerator ResetTargetAcquiredFlag()
    {
        yield return new WaitForSeconds(2f);
        SpaceshipAudioController.stopTargetAquiredSound = false;
    }
}
