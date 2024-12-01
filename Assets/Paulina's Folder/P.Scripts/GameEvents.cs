using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Scriptable Objects/GameEvents")]
public class GameEvents : ScriptableObject
{
    public delegate void UpdateQuestionUICallback(Question question);
    public UpdateQuestionUICallback updateQuestionUI;

    public delegate void UpdateQuestionAnswerCalback(AnswerData pickedAnswer);
    public UpdateQuestionAnswerCalback updateQuestionAnswer;

    public delegate void DisplayResolutionScreenCallback(UIManager.ResolutionScreenType type, int score);
    public DisplayResolutionScreenCallback displayResolutionScreen;

    public delegate void ScoreUpdatedCallback();
    public ScoreUpdatedCallback scoreUpdated;

    [HideInInspector]
    public int CurrentFinalScore;

}
