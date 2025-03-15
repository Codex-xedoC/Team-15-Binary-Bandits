using UnityEngine;
using UnityEngine.SceneManagement; 
public class OuterSpaceGameManager : MonoBehaviour
{
    public static OuterSpaceGameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            if (MainMenuHandler.Instance != null)
            {
                MainMenuHandler.Instance.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("MainMenuHandler instance not found! Cannot load scene.");
            }
        }
        else
        {
            Debug.LogError("Scene name is empty or null.");
        }
    }
}
