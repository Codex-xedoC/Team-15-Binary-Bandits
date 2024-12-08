using UnityEngine;

public class QuestionZoneScript : MonoBehaviour
{

    public MountainLevelScript mountainLevelScript;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !mountainLevelScript.questionDisplayed)
        {
            mountainLevelScript.GetRandomQuestion();
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
