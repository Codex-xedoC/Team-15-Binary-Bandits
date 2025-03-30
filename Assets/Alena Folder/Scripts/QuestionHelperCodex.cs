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

    private List<Question> questions = new List<Question>();
    private Question currentQuestion;
    private GameObject lastActivePanel;

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

        switch (currentQuestion.QuestionType)
        {
            case "Multiple Choice":
                MultipleChoice.SetActive(true);
                MultipleChoiceQuestionText.text = "Question: " + currentQuestion.QuestionText;
                MultipleChoiceDropdown.ClearOptions();
                MultipleChoiceDropdown.AddOptions(new List<string>(currentQuestion.Choices));
                PositionPanel(MultipleChoice);
                lastActivePanel = MultipleChoice;
                break;

            case "True/False":
                TrueFalse.SetActive(true);
                TrueFalseQuestionText.text = "Question: " + currentQuestion.QuestionText;
                TrueFalseDropdown.ClearOptions();
                TrueFalseDropdown.AddOptions(new List<string> { "True", "False" });
                PositionPanel(TrueFalse);
                lastActivePanel = TrueFalse;
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
                PositionPanel(ImageQuestion);
                lastActivePanel = ImageQuestion;
                break;

            default:
                Debug.LogWarning("Unsupported question type: " + currentQuestion.QuestionType);
                break;
        }

        Debug.Log("[QuestionHelperCodex] Displayed new question.");
    }

    private void PositionPanel(GameObject panel)
    {
        Transform cam = Camera.main.transform;
        Vector3 spawnPos = cam.position + cam.forward * 3f;

        panel.transform.SetParent(null);
        panel.transform.position = spawnPos;
        panel.transform.rotation = Quaternion.LookRotation(cam.forward, cam.up);

        if (BackPanel != null)
        {
            BackPanel.transform.SetParent(null);
            BackPanel.transform.localScale = Vector3.one;
            BackPanel.transform.rotation = panel.transform.rotation;

            Vector3 offset = panel.transform.right * -3f;
            BackPanel.transform.position = panel.transform.position + offset;

            BackPanel.SetActive(true);
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
        {
            BackPanel.SetActive(false);
        }
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

        bool isCorrect = selected == currentQuestion.CorrectAnswer;
        StartCoroutine(ShowResultPanel(isCorrect));
    }

    private IEnumerator ShowResultPanel(bool isCorrect)
    {
        yield return new WaitForSeconds(0.2f);

        GameObject resultPanel = isCorrect ? CorrectPanel : WrongPanel;
        resultPanel.SetActive(true);

        if (lastActivePanel != null)
        {
            resultPanel.transform.position = lastActivePanel.transform.position;
            resultPanel.transform.rotation = lastActivePanel.transform.rotation;
        }

        resultPanel.transform.localScale = Vector3.one * 1.5f;

        if (isCorrect)
        {
            XRShipHealth.Instance.AddScore(10);
        }

        yield return new WaitForSeconds(3f);
        resultPanel.SetActive(false);
    }
}
