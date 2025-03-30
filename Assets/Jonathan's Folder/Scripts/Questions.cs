
[System.Serializable]
public class Questions
{
    /*
      public Questions(string[] answers)
      {
          Choices = answers;
      }
      */

    public int index;
    public string QNumber;
    public string QuestionText;
    public string[] Choices;
    public string CorrectAnswer;
    public string QuestionType;


}
