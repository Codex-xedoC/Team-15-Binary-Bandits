using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class TutorialCardManager : MonoBehaviour
{
    [Header("Tutorial Cards (in order)")]
    public List<GameObject> TutorialCards;

    [Header("UI")]
    public TMP_Text ContinueButtonText;
    public Button ContinueButton;

    private int currentCardIndex = 0;
    bool buttonCanBePressed = true; // used to stop duplicate calls in fractions of a second

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
        if (buttonCanBePressed) // Checks to make sure the button has not been called in the last second
        {
            StartCoroutine(ButtonDelay()); // Start 1 second delay

            currentCardIndex++;

            if (currentCardIndex >= TutorialCards.Count)
            {
                currentCardIndex = 0;
            }

            ShowCard(currentCardIndex);
        }
    }

    // Give the button a delay because it was getting double clicking
    IEnumerator ButtonDelay()
    {

        buttonCanBePressed = false;
        yield return new WaitForSeconds(1);
        buttonCanBePressed = true;
    }
}
