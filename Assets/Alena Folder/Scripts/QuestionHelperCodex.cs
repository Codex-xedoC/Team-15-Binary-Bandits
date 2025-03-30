using UnityEngine;
using UnityEngine.UI;
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

    private List<Question> questions = new List<Question>();
    private Question currentQuestion;

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
                break;

            case "True/False":
                TrueFalse.SetActive(true);
                TrueFalseQuestionText.text = "Question: " + currentQuestion.QuestionText;
                TrueFalseDropdown.ClearOptions();
                TrueFalseDropdown.AddOptions(new List<string> { "True", "False" });
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
                    Debug.LogWarning($"Image Q{currentQuestion.QNumber} not found in Resources.");
                }

                ImageQuestionDropdown.ClearOptions();
                ImageQuestionDropdown.AddOptions(new List<string>(currentQuestion.Choices));
                break;

            default:
                Debug.LogWarning("Unsupported question type: " + currentQuestion.QuestionType);
                break;
        }

        Debug.Log("[QuestionHelperCodex] Displayed new question.");
    }

    private void HideAllPanels()
    {
        MultipleChoice.SetActive(false);
        TrueFalse.SetActive(false);
        ImageQuestion.SetActive(false);
        CorrectPanel.SetActive(false);
        WrongPanel.SetActive(false);
    }

    public void SubmitAnswer()
    {
        string selected = "";

        if (currentQuestion.QuestionType == "Multiple Choice")
        {
            selected = MultipleChoiceDropdown.options[MultipleChoiceDropdown.value].text;
        }
        else if (currentQuestion.QuestionType == "True/False")
        {
            selected = TrueFalseDropdown.options[TrueFalseDropdown.value].text;
        }
        else if (currentQuestion.QuestionType == "Image Question")
        {
            selected = ImageQuestionDropdown.options[ImageQuestionDropdown.value].text;
        }

        if (selected == currentQuestion.CorrectAnswer)
        {
            CorrectPanel.SetActive(true);
            WrongPanel.SetActive(false);
        }
        else
        {
            WrongPanel.SetActive(true);
            CorrectPanel.SetActive(false);
        }

        Invoke(nameof(HideAllPanels), 3f);
    }
}
