using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    //public int inputScore = 0;
    //MainMenuHandler mainMenuHandler;

    public UnityEvent<int> submitScoreEvent;

    public void SubmitScore()
    {

        submitScoreEvent.Invoke(MainMenuHandler.Instance.numCorrect);
    }
}
