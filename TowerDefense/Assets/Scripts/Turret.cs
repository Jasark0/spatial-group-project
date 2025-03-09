using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 10f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform[] firePoints;
    public float health = 100f;
    public float damagePerSecond;

    Balloon currentTarget = null;
    float timeOfLastAttack;

    // Upgrade variables
    public int fireRateLevel = 1;
    public int healthLevel = 1;
    private GameManager gameManager;

    public Vector3 canvasOffset = new Vector3(0, 2, 0);

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        timeOfLastAttack = Time.time;
    }

    void Update()
    {
        if (currentTarget)
        {
            RotateTowardsTarget(currentTarget);
            Shoot(currentTarget);
        }
        else
        {
            currentTarget = FindClosestEnemy();
        }
    }

    Balloon FindClosestEnemy()
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

        return closest != null && minDistance <= range ? closest.GetComponent<Balloon>() : null;
    }

    void RotateTowardsTarget(Balloon target)
    {
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    void Shoot(Balloon target)
    {
        if (Time.time - timeOfLastAttack > fireRate && bulletPrefab && firePoints.Length > 0)
        {
            foreach (Transform firePoint in firePoints)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                if (bullet.TryGetComponent<Bullet>(out var bulletScript))
                {
                    Vector3 direction = target.transform.position - firePoint.position;
                    bulletScript.Init(direction);
                }
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

    public void OnMouseDown()
    {
    FindObjectOfType<UpgradePanelManager>().ShowUpgradePanel(this);
    }

    public void UpgradeFireRate()
    {
        fireRateLevel++;
        fireRate *= 0.9f;
    }

    public void UpgradeHealth()
    {
        healthLevel++;
        health += 5f;
    }
}

