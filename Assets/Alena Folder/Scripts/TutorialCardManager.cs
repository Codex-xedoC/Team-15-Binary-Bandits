using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TutorialCardManager : MonoBehaviour
{
    [Header("Tutorial Cards (in order)")]
    public List<GameObject> TutorialCards;

    [Header("UI")]
    public TMP_Text ContinueButtonText;
    public Button ContinueButton;

    private int currentCardIndex = 0;

    void Start()
    {
        ShowCard(currentCardIndex);

        if (ContinueButton != null)
            ContinueButton.onClick.AddListener(AdvanceCard);
    }

    void ShowCard(int index)
    {
        for (int i = 0; i < TutorialCards.Count; i++)
        {
            TutorialCards[i].SetActive(i == index);
        }

        if (index >= 0 && index < TutorialCards.Count)
        {
            ContinueButtonText.text = (index == TutorialCards.Count - 1) ? "Restart" : "Continue";
        }
    }

    public void AdvanceCard()
    {
        currentCardIndex++;

        if (currentCardIndex >= TutorialCards.Count)
        {
            currentCardIndex = 0;
        }

        ShowCard(currentCardIndex);
    }
}
