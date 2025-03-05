using UnityEngine;

public class MainTower : MonoBehaviour
{
    public float health = 100f;

    void Start()
    {
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject); 
        }
    }
}
