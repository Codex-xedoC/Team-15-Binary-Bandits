using UnityEngine;

public class StartButtonScript : MonoBehaviour
{
    // Reference to Aaron's SceneManager
    public SceneManager sceneManager;

    public void StartGame()
    {
        // Call CodexLevelPressed() from Aaron's SceneManager to load the CodexScene
        if (sceneManager != null)
        {
            sceneManager.CodexLevelPressed();
        }
        else
        {
            Debug.LogError("SceneManager is not assigned!");
        }
    }
}
