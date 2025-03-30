using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public FadeScreen fadeScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fadeScreen = GameObject.Find("Fader Screen").GetComponent<FadeScreen>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToScene(string sceneName)
    {
        StartCoroutine(GoToSceneRoutine(sceneName));
    }

    IEnumerator GoToSceneRoutine(string sceneName)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void ReturnMainMenu()
    {
        GoToScene("MainMenu");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void AaronsLevelPressed()
    {
        GoToScene("AaronsLevel");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("AaronsLevel");
    }

    public void LinaLevelPressed()
    {
        GoToScene("City Game");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("City Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void JulianLevelPressed()
    {
        GoToScene("MountainClimb");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("MountainClimb");

    }

    public void CodexLevelPressed()
    {
        GoToScene("CodexScene");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("CodexScene");
    }

    public void JonathanLevelPressed()
    {
        GoToScene("Maze1");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Maze1");
    }
}
