using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    [Header("Answer Buttons")]
    public List<TextMeshProUGUI> MultipleChoiceAnswers;
    public List<TextMeshProUGUI> ImageQuestionAnswers;
    public List<TextMeshProUGUI> TrueFalseAnswers;

    [Header("Question Texts")]
    public TextMeshProUGUI MultipleChoiceQuestionText;
    public TextMeshProUGUI TrueFalseQuestionText;
    public TextMeshProUGUI ImageQuestionText;

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
            return;

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
    }

    public void DisplayNewQuestion()
    {
        if (questions.Count == 0)
            return;

        currentQuestion = questions[Random.Range(0, questions.Count)];

        HideAllPanels();
        GameObject activePanel = null;

        switch (currentQuestion.QuestionType)
        {
            case "Multiple Choice":
                MultipleChoice.SetActive(true);
                MultipleChoiceQuestionText.text = "Question: " + currentQuestion.QuestionText;
                for (int i = 0; i < MultipleChoiceAnswers.Count; i++)
                    MultipleChoiceAnswers[i].text = currentQuestion.Choices[i];
                activePanel = MultipleChoice;
                break;

            case "True/False":
                TrueFalse.SetActive(true);
                TrueFalseQuestionText.text = "Question: " + currentQuestion.QuestionText;
                TrueFalseAnswers[0].text = currentQuestion.Choices[0];
                TrueFalseAnswers[1].text = currentQuestion.Choices[1];
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

                for (int i = 0; i < ImageQuestionAnswers.Count; i++)
                    ImageQuestionAnswers[i].text = currentQuestion.Choices[i];

                activePanel = ImageQuestion;
                break;
        }

        if (activePanel != null)
        {
            PositionPanel(activePanel);
            lastActivePanel = activePanel;
        }
    }

    private void PositionPanel(GameObject panel)
    {
        Transform cam = Camera.main != null ? Camera.main.transform : player.transform;

        if (cam == null)
            return;

        Vector3 spawnPos = cam.position + cam.forward * 2.5f;

        panel.transform.position = spawnPos;
        panel.transform.rotation = Quaternion.LookRotation(cam.forward, cam.up);
        panel.transform.localScale = Vector3.one * 0.005f;

        lastQuestionPosition = spawnPos;
        lastQuestionRotation = panel.transform.rotation;

        CanvasGroup group = panel.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        panel.SetActive(true);

        if (BackPanel != null)
        {
            BackPanel.transform.position = spawnPos + panel.transform.right * -2f;
            BackPanel.transform.rotation = panel.transform.rotation;
            BackPanel.transform.localScale = Vector3.one;

            CanvasGroup backGroup = BackPanel.GetComponent<CanvasGroup>();
            if (backGroup != null)
            {
                backGroup.alpha = 1f;
                backGroup.interactable = true;
                backGroup.blocksRaycasts = true;
            }

            BackPanel.SetActive(true);
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

    public void SubmitAnswer(TextMeshProUGUI selectedText)
    {
        string selected = selectedText.text.Trim();
        bool isCorrect = selected.Equals(currentQuestion.CorrectAnswer.Trim(), System.StringComparison.OrdinalIgnoreCase);

        if (currentQuestion.QuestionType == "Multiple Choice")
            MultipleChoice.SetActive(false);
        else if (currentQuestion.QuestionType == "True/False")
            TrueFalse.SetActive(false);
        else if (currentQuestion.QuestionType == "Image Question")
            ImageQuestion.SetActive(false);

        if (isCorrect)
        {
            ShowPanelImmediate(CorrectPanel);
            XRShipHealth.Instance.Refuel();
            MainMenuHandler.Instance.questionCorrect();
            XRShipHealth.Instance.AddCorrect();
            ScoreManager sm = FindObjectOfType<ScoreManager>();
            if (sm != null) sm.SubmitScore();
        }
        else
        {
            ShowPanelImmediate(WrongPanel);
            MainMenuHandler.Instance.questionWrong();
            XRShipHealth.Instance.AddWrong();
        }
    }

    private void ShowPanelImmediate(GameObject panel)
    {
        if (panel == null)
            return;

        Transform t = panel.transform;
        while (t != null)
        {
            t.gameObject.SetActive(true);
            t = t.parent;
        }

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
