using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class QuestionHelperCodex : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public GameObject CorrectUI, DamageTextUI;

    [Header("Answer Buttons")]
    public GameObject ButtonA, ButtonB, ButtonC;

    private string correctAnswer;
    private List<string> questionsAndAnswers = new List<string>();

    void Start()
    {
        LoadQuestionsFromFile();
        DisplayNewQuestion(); // ? Now accessible from other scripts
    }

    private void LoadQuestionsFromFile()
    {
        TextAsset questionFile = Resources.Load<TextAsset>("computer_science_questions");

        if (questionFile == null)
        {
            Debug.LogError("? Question file not found!");
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

                int colonIndex = line.IndexOf(":");
                questionEntry = (colonIndex != -1 && colonIndex + 1 < line.Length)
                    ? line.Substring(colonIndex + 1).Trim()
                    : line;
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

    // ? **Now Public: Can Be Called from `PlanetInteraction.cs`**
    public void DisplayNewQuestion()
    {
        if (questionsAndAnswers.Count == 0)
        {
            Debug.LogError("? No questions loaded!");
            return;
        }

        int randomIndex = Random.Range(0, questionsAndAnswers.Count);
        string randomQuestionEntry = questionsAndAnswers[randomIndex];

        string[] parts = randomQuestionEntry.Split(new[] { "Answer:" }, System.StringSplitOptions.None);

        if (parts.Length < 2)
        {
            Debug.LogError("? Invalid question format!");
            return;
        }

        questionText.text = parts[0].Trim();
        correctAnswer = parts[1].Trim();
    }

    public void SelectA() => CheckAnswer("A");
    public void SelectB() => CheckAnswer("B");
    public void SelectC() => CheckAnswer("C");

    private void CheckAnswer(string selectedAnswer)
    {
        ButtonA.SetActive(false);
        ButtonB.SetActive(false);
        ButtonC.SetActive(false);

        if (selectedAnswer == correctAnswer)
        {
            StartCoroutine(CorrectAnswerFeedback());
        }
        else
        {
            StartCoroutine(WrongAnswerFeedback());
        }
    }

    private IEnumerator CorrectAnswerFeedback()
    {
        CorrectUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        CorrectUI.SetActive(false);
        RestartQuestion();
    }

    private IEnumerator WrongAnswerFeedback()
    {
        DamageTextUI.SetActive(true);
        XRShipHealth.Instance.TakeDamage(10);
        yield return new WaitForSeconds(3f);
        DamageTextUI.SetActive(false);
        RestartQuestion();
    }

    private void RestartQuestion()
    {
        ButtonA.SetActive(true);
        ButtonB.SetActive(true);
        ButtonC.SetActive(true);
        DisplayNewQuestion();
    }
}
