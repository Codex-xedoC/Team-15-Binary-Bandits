
using NUnit.Framework.Internal;
using System.Diagnostics;

[System.Serializable]
public class Questions
{
    public string QNumber;
    public string QuestionText;
    public string[] Choices;
    /*
    public Question(string[] answers)
    {
        Choices = answers;
    }
    */
    public string CorrectAnswer;
    public string QuestionType;


}
