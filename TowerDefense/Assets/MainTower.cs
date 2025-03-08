using UnityEngine;

public class MainTower : MonoBehaviour
{
    public float health = 100f;
    public GameObject deathScreen;

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
        Time.timeScale = 0f;
    }
}
