using UnityEngine;

public class HighlightFunc : MonoBehaviour
{
    public GameObject highlight;
    
    public void HoverEnter()
    {
        highlight.SetActive(true);
    }

    public void HoverExit()
    {
        highlight.SetActive(false);
    }

}
