using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class AstroidCollision : MonoBehaviour
{
    public TextMeshProUGUI health;
    int HP = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.nKey.wasReleasedThisFrame)
        {
            HP--;
            if (HP == 0)
                UnityEngine.SceneManagement.SceneManager.LoadScene("CodexLevel");
            else
                health.text = "Health: " + HP;
        }

    }
}
