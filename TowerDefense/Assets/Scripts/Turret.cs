using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 10f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float health = 100f;
    public float damagePerSecond;
    Enemy currentTarget = null;
    float timeOfLastAttack;

    void Start()
    {
        timeOfLastAttack = Time.time;
    }
    void Update()
    {
        if (currentTarget)
            Shoot(currentTarget);
        else
            currentTarget = FindClosestEnemy();

    }

    Enemy FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest != null && minDistance <= range ? closest.GetComponent<Enemy>() : null;
    }

    void Shoot(Enemy target)
    {
        if (Time.time - timeOfLastAttack > fireRate && bulletPrefab && firePoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            if (bullet.TryGetComponent<Bullet>(out var bulletScript))
            {
                Vector3 direction = target.transform.position - firePoint.position;
                bulletScript.Init(direction);
            }
            timeOfLastAttack = Time.time;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
