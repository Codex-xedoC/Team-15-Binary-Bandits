
[System.Serializable]
public class QuestionAnswerBank 
{
    public string Question;
    public string[] Answers;

    public QuestionAnswerBank(string[] answers)
    {
        Answers = answers;
    }

    public int CorrectAnswer;

   

}
