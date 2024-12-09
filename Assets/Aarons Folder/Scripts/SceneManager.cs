using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void AaronsLevelPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("AaronsLevel");
    }

    public void LinaLevelPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("City Game");
    }

    public void JulianLevelPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MountainClimb");

    }

    public void CodexLevelPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("CodexScene");
    }

    public void JonathanLevelPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Maze1");
    }
}
