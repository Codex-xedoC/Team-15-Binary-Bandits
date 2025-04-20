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

    [Header("Question Display Panel (Left Side)")]
    public GameObject QuestionPanelUI;

    [Header("Player Camera")]
    public Transform cameraRig;

    private List<Question> questions = new List<Question>();
    private Question currentQuestion;
    private GameObject currentAnswerPanel;

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
        if (cameraRig == null && Camera.main != null)
            cameraRig = Camera.main.transform;

        LoadQuestions();
    }

    private void LoadQuestions()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("QuestionBank");
        if (csvFile == null) return;

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
        if (questions.Count == 0) return;

        currentQuestion = questions[Random.Range(0, questions.Count)];
        HideAllPanels();
        ResetAnswerColors();

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
            currentAnswerPanel = activePanel;
            PositionAllPanels();
        }
    }

    private void PositionAllPanels()
    {
        if (cameraRig == null) return;

        Vector3 centerPos = cameraRig.position + cameraRig.forward * 5.5f;

        // lock to Y-axis only to keep upright
        Quaternion uprightRotation = Quaternion.Euler(0, cameraRig.eulerAngles.y, 0);

        // Question Panel Left
        if (QuestionPanelUI != null)
        {
            QuestionPanelUI.transform.SetParent(null);
            QuestionPanelUI.transform.position = centerPos + -cameraRig.right * 2.5f;
            QuestionPanelUI.transform.rotation = uprightRotation;
            QuestionPanelUI.transform.localScale = Vector3.one;
            EnableCanvasGroup(QuestionPanelUI);
        }

        // Answer Panel Right
        if (currentAnswerPanel != null)
        {
            currentAnswerPanel.transform.SetParent(null);
            currentAnswerPanel.transform.position = centerPos + cameraRig.right * 2.5f;
            currentAnswerPanel.transform.rotation = uprightRotation;
            currentAnswerPanel.transform.localScale = Vector3.one;
            EnableCanvasGroup(currentAnswerPanel);
        }

        // Menu Button Center
        if (BackPanel != null)
        {
            BackPanel.transform.SetParent(null);
            BackPanel.transform.position = centerPos;
            BackPanel.transform.rotation = uprightRotation;
            BackPanel.transform.localScale = Vector3.one;
            EnableCanvasGroup(BackPanel);
        }
    }

    public void SubmitAnswer(TextMeshProUGUI selectedText)
    {
        string selected = selectedText.text.Trim();
        bool isCorrect = selected.Equals(currentQuestion.CorrectAnswer.Trim(), System.StringComparison.OrdinalIgnoreCase);

        if (isCorrect)
        {
            currentAnswerPanel.SetActive(false);
            ShowFeedback(CorrectPanel);
            XRShipHealth.Instance.Refuel();
            MainMenuHandler.Instance.questionCorrect();
            XRShipHealth.Instance.AddCorrect();
            ScoreManager sm = FindObjectOfType<ScoreManager>();
            if (sm != null) sm.SubmitScore();
        }
        else
        {
            HighlightCorrectAnswer();
            ShowFeedback(WrongPanel);
            MainMenuHandler.Instance.questionWrong();
            XRShipHealth.Instance.AddWrong();
        }
    }

    private void ShowFeedback(GameObject panel)
    {
        if (panel == null || cameraRig == null) return;
        panel.transform.SetParent(null);
        panel.transform.position = cameraRig.position + cameraRig.forward * 3f;
        panel.transform.rotation = Quaternion.Euler(0, cameraRig.eulerAngles.y, 0);
        panel.transform.localScale = Vector3.one;
        EnableCanvasGroup(panel);
    }

    private void HighlightCorrectAnswer()
    {
        List<TextMeshProUGUI> answers = null;

        switch (currentQuestion.QuestionType)
        {
            case "Multiple Choice": answers = MultipleChoiceAnswers; break;
            case "True/False": answers = TrueFalseAnswers; break;
            case "Image Question": answers = ImageQuestionAnswers; break;
        }

        if (answers != null)
        {
            foreach (var a in answers)
            {
                if (a.text.Trim().Equals(currentQuestion.CorrectAnswer.Trim(), System.StringComparison.OrdinalIgnoreCase))
                {
                    a.color = Color.green;
                }
            }
        }
    }

    private void ResetAnswerColors()
    {
        foreach (var a in MultipleChoiceAnswers) a.color = Color.white;
        foreach (var a in TrueFalseAnswers) a.color = Color.white;
        foreach (var a in ImageQuestionAnswers) a.color = Color.white;
    }

    private void HideAllPanels()
    {
        MultipleChoice.SetActive(false);
        TrueFalse.SetActive(false);
        ImageQuestion.SetActive(false);
        CorrectPanel.SetActive(false);
        WrongPanel.SetActive(false);
        if (QuestionPanelUI != null) QuestionPanelUI.SetActive(false);
        if (BackPanel != null) BackPanel.SetActive(false);
    }

    private void EnableCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
        obj.SetActive(true);
    }

    private IEnumerator ResetTargetAcquiredFlag()
    {
        yield return new WaitForSeconds(2f);
        SpaceshipAudioController.stopTargetAquiredSound = false;
    }
}
