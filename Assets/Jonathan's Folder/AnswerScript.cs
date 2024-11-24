using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnswerScript : MonoBehavior
{
    public bool isCorrect = false;
    public void Answer()
    {
        if (isCorrect)
        {
            Debug.Log("Correct Answer");
        }
        else
        {
            Debug.Log("Wrong Answer");
        }
}
