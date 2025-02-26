using UnityEngine;
using TMPro;

public class AsteroidCollision : MonoBehaviour
{
    public TextMeshProUGUI HealthText;  // Use HealthText instead of HealthUI
    public TextMeshProUGUI DamageText;

    private int health = 100;

    void Start()
    {
        if (HealthText == null)
        {
            Debug.LogError("?? HealthText is NOT assigned! Drag your HealthText UI to the script.");
        }
        if (DamageText == null)
        {
            Debug.LogError("?? DamageText is NOT assigned! Drag your DamageText to the script.");
        }
        DamageText.gameObject.SetActive(false); // Hide it initially
        UpdateHealthUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid"))
        {
            TakeDamage(5);  // Lose 5 health per asteroid hit
            Destroy(other.gameObject);
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            Debug.Log("?? Game Over! Health reached 0.");
            // Implement Game Over logic here
        }

        ShowDamageText(damage);
        UpdateHealthUI();
    }

    void ShowDamageText(int damage)
    {
        if (DamageText != null)
        {
            DamageText.text = $"-{damage} Health";
            DamageText.color = Color.red;
            DamageText.gameObject.SetActive(true);

            CancelInvoke(nameof(HideDamageText));
            Invoke(nameof(HideDamageText), 1.5f);
        }
    }

    void HideDamageText()
    {
        DamageText.gameObject.SetActive(false);
    }

    void UpdateHealthUI()
    {
        if (HealthText != null)
        {
            HealthText.text = $"Health: {health}";
        }
    }
}
