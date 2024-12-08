using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuScript : MonoBehaviour
{

    public GameObject menuPanel;
    public InputActionReference toggleMenuAction;

    private bool isMenuVisible = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (toggleMenuAction.action.triggered)
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        isMenuVisible = !isMenuVisible;
        menuPanel.SetActive(isMenuVisible);
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect"); //Load level select scene
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Mountaiin-Climbing"); //Restart minigame
    }

    public void LoadStatistics()
    {
        SceneManager.LoadScene("Statistics");   //Load stats screen
    }
}
