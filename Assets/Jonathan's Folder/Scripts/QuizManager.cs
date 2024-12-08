using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class QuizManager : MonoBehaviour
{
    public List<QuestionAnswerBank> QnA;
    public List<Vector3> coords;
    public GameObject[] options;
    public int currentQuestion;
    //public bool flag;
    public Text QuestionTxt;
    public Text timerText;
    public GameObject Quizpanel;
    public GameObject GoPanel;
    public GameObject ParentPanel;
    public Text wrongAnswers;
    public int panelNum = 0;
    public Text Time;
    public int wscore = 0;
    int totalQuestions = 0;
    public GameObject XRRig;
    private void Start()
    {
        totalQuestions = QnA.Count;
        GoPanel.SetActive(false);
        generateQuestion();
    }

    public void goBack()
    {
        XRRig.transform.position = new Vector3(0, 0.97f, 1);
    }
    void GameOver()
    {
        Time = timerText;
        panelNum = 0;
        //wrongAnswers.text = wscore.ToString();
        Quizpanel.SetActive(false);
        GoPanel.SetActive(true);
        GoPanel.transform.GetChild(0).GetComponent<Text>().text = Time.text;
    }
    public void correct()
    {

        if (panelNum <= 5) //5
        {
            ParentPanel.transform.position = coords[panelNum];
            panelNum++;
            QnA.RemoveAt(currentQuestion);
            generateQuestion();
        }
        else
        {
            GameOver();
        }

    }

    public void retry()
    {
        goBack();
        timerText.text = "{0:00}";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Maze1");
    }

    public void quit()
    {
        Application.Quit();
    }
    public void wrong()
    {
        wscore += 1;
        QnA.RemoveAt(currentQuestion);
        generateQuestion();
    }

    void setAnswers()
    {
        for (int i = 0; i < options.Length; i++)
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