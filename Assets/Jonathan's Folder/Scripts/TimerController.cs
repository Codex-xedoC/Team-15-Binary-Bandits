
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    [SerializeField] private float timeCounter;
    [SerializeField] private int min;
    [SerializeField] private int sec;
    [SerializeField] private Text timerText;
    [SerializeField] public bool stop = false;

    private void Update()
    {
        if (!stop)
        {
            timeCounter += Time.deltaTime;
            min = Mathf.FloorToInt(timeCounter / 60f);
            sec = Mathf.FloorToInt(timeCounter - min * 60);
            timerText.text = string.Format("{0:00}:{1:00}", min, sec);
        }
    }

}
