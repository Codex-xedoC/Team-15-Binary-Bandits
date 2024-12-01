using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    Question[] questions = null;
    public Question[] Questions { get { return questions; } }

    [SerializeField] GameEvents events = null;

    private List<AnswerData> PickedAnswers = new List<AnswerData>();
    private List<int> FinishedQuestions = new List<int>();
    private int currentQuestion = 0;

    void Start () 
    {
        
        LoadQuestions();

        foreach (var question in Questions)
        {
            Debug.Log(question.Info);
        }

       Display();

    }

    public void eraseAnswers()
    {
        PickedAnswers = new List<AnswerData>();
    }

    void Display()
    {
        eraseAnswers();
        var question = GetRandomQuestion();

        if (events.updateQuestionUI != null)
        {
            events.updateQuestionUI(question);
        } else { Debug.LogWarning("Something went wrong while trying to display new Question UI Data. GameEvents.UpdateQuestionUI is null. Issue in GameManager.Display() method."); }
    }

    Question GetRandomQuestion()
    {
        var randomIndex = GetRandomQuestionIndex();
        currentQuestion = randomIndex;

        return Questions[currentQuestion];
    }

    int GetRandomQuestionIndex()
    {
        var random = 0;
        if (FinishedQuestions.Count < Questions.Length)
        {
            do
            {
                random = UnityEngine.Random.Range(0, Questions.Length);
            } while (FinishedQuestions.Contains(random) || random == currentQuestion);
        }
        return random;
    }

    void LoadQuestions()
    {
        Object[] objects = Resources.LoadAll("Questions", typeof(Question));
        questions = new Question[objects.Length];
        
        for (int i = 0; i < objects.Length; i++)
        {
            questions[i] = (Question)objects[i];
        }
    }
}
