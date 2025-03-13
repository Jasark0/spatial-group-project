using UnityEngine;
using TMPro;

public class MainTower : MonoBehaviour
{
    public float health = 100f;
    public GameObject deathScreen;
    public TMP_Text scoreText;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    } 

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
            Destroy(gameObject); 
        }
    }

        private void Die()
    {
        deathScreen.SetActive(true);
        if (scoreText != null && gameManager != null)
        {
            scoreText.text = "Final Score: " + gameManager.score;
        }
        Time.timeScale = 0f;
    }
}
