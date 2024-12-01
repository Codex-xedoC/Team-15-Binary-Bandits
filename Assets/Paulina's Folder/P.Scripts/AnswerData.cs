using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnswerData : MonoBehaviour
{
    [Header("UI Element")]
    [SerializeField] TextMeshProUGUI infoTextObj;
    [SerializeField] Image toggle;

    [Header("Textures")]
    [SerializeField] Sprite uncheckedToggle;
    [SerializeField] Sprite checkedToggle;

    private RectTransform rect;
    public RectTransform Rect
    {
        get
        {
            if(rect == null)
            {
                rect = GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
            }
            return rect;
        }
    }

    [Header("References")]
    [SerializeField] GameEvents events;

    private bool Checked = false;

    private int answerIndex = -1;
    public int AnswerIndex { get { return answerIndex; } }

    public void UpdateData (string info, int index)
    {
        infoTextObj.text = info;
        answerIndex = index;
    }

    public void Reset()
    {
        Checked = false;
        UpdateUI();
    }

    public void SwitchState() 
    {
        Checked = !Checked;
        UpdateUI();

        if (events.updateQuestionAnswer != null)
        {
            events.updateQuestionAnswer(this);
        }
    }

    void UpdateUI() 
    {
        toggle.sprite = (Checked) ? checkedToggle : uncheckedToggle;
    }

    
}
