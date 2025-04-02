using UnityEngine;
using UnityEngine.SceneManagement;

public class OuterSpaceGameManager : MonoBehaviour
{
    public static OuterSpaceGameManager instance;

    [Header("Audio")]
    public AudioSource engineAudioSource;
    public string engineSceneName = "Codex";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SubmitFinalScoreBeforeExit();

            if (MainMenuHandler.Instance != null)
            {
                MainMenuHandler.Instance.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("MainMenuHandler instance not found! Cannot load scene.");
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName); 
            }
        }
        else
        {
            Debug.LogError("Scene name is empty or null.");
        }
    }

    private void SubmitFinalScoreBeforeExit()
    {
        if (XRShipHealth.Instance != null)
        {
            XRShipHealth.Instance.SubmitScoreExternally();
            Debug.Log("[OuterSpaceGameManager] Score submitted before exiting scene.");
        }
        else
        {
            Debug.LogWarning("[OuterSpaceGameManager] XRShipHealth not found. Score not submitted.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == engineSceneName)
        {
            Debug.Log("[OuterSpaceGameManager] Loaded space exploration scene.");

            if (engineAudioSource != null && !engineAudioSource.isPlaying)
            {
                engineAudioSource.Play();
                Debug.Log("[OuterSpaceGameManager] Engine audio started.");
            }
            else if (engineAudioSource == null)
            {
                Debug.LogWarning("[OuterSpaceGameManager] Engine AudioSource not assigned.");
            }
        }
    }
}
