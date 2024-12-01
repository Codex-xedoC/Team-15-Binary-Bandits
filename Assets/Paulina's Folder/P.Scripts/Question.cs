using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct Answer 
{
    [SerializeField] private string info;
    public string Info { get { return info; } }

    [SerializeField] private bool isCorrect;
    public bool IsCorrect { get { return isCorrect; } }
}

[CreateAssetMenu(fileName = "Question", menuName = "Scriptable Objects/Question")]
public class Question : ScriptableObject
{
    [SerializeField] private string info = string.Empty;
    public string Info { get { return info; } }

    [SerializeField] Answer[] answers = null;

    public Answer[] Answers { get { return answers; } }

    //Parameter
    [SerializeField] private int addScore = 1;
    public int AddScore { get { return addScore; } }

    public List<int> GetCorrectAnswers()
    {
        List<int> CorrectAnswers = new List<int>();
         for (int i = 0; i < Answers.Length; i++)
         {
            if (Answers[i].IsCorrect)
            {
                CorrectAnswers.Add(i);
            
            }
         }
         return CorrectAnswers;
    }
}
