using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> names;
    [SerializeField]
    private List<TextMeshProUGUI> scores;

    private string publicLeaderboardKey = "8db98953379f11bee83e64b0057fdc2a491d80d5675294ceced64d7bda479675";

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg) =>
        {
            int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;
            for (int i = 0; i < loopLength; i++)
            {
                names[i].text = msg[i].Username;
                scores[i].text = msg[i].Score.ToString();
            }
        }));
    }

    public void SetLeaderboardEntry(string username, int score)
    {
        if (username == null)
            username = "temp";
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, ((msg) =>
        {
            GetLeaderboard();
        }));
    }
}
