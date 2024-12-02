using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


public class QuizManager : MonoBehaviour
{
    public List<QuestionAnswerBank> QnA;
    public GameObject[] options;
    public int currentQuestion;
    //public bool flag;
    public Text QuestionTxt;
    public GameObject Quizpanel;
    public GameObject GoPanel;
    public GameObject ParentPanel;
    //public int panelNum = 0;
    public Text Time;
    public int wscore = 0;
    int totalQuestions = 0;
    private void Start()
    {
        totalQuestions = QnA.Count;
        GoPanel.SetActive(false);
        generateQuestion();
    }
    

    void GameOver()
    {
        Quizpanel.SetActive(false);
        GoPanel.SetActive(true);
        //Time.text = "Test";
    }
    public void correct()
    {
        ParentPanel.transform.position = new Vector3(6.22f, 0.97f, 16.21f);
        //Quizpanel.SetActive(false);
        QnA.RemoveAt(currentQuestion);
        generateQuestion();
        
    }

    public void retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void wrong()
    {
        wscore += 1;
        QnA.RemoveAt(currentQuestion);
        generateQuestion();
    }

    void setAnswers()
    {
        for(int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            
           // options[i].transform.GetChild(0).GetComponent<TextMeshPro>().text
           //     = QnA[currentQuestion].Answers[i];
            options[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text
                = QnA[currentQuestion].Answers[i];


            if (QnA[currentQuestion].CorrectAnswer == i + 1)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = true;
                
            }

        }
    }

    void generateQuestion()
    {
        if (QnA.Count > 0)
        {
            currentQuestion = Random.Range(0, QnA.Count);
            QuestionTxt.text = QnA[currentQuestion].Question;

            setAnswers();
        }
        else
        {
            GameOver();
            Debug.Log("Out of Questions");
        }
    }

}
