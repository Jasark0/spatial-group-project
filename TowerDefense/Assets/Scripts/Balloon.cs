using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public int health = 10;
    public float speed = 0.5f;
    private GameManager gameManager;
    public Animator animator;
    private bool hasExploded = false;
    public GameObject explosionPrefab;
    public float explosionDamage = 5f;
    private Transform target;

    protected virtual void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();

        GameObject tower = GameObject.FindGameObjectWithTag("MainTower");
        if (tower != null)
        {
            target = tower.transform;
        }
        else
        {
            Debug.LogError("MainTower not found in the scene!");
            Destroy(gameObject);
        }
    }

    protected virtual void Update()
    {
        if (!hasExploded && target != null)
        {
            MoveTowardsTarget();
        }
    }

private void OnTriggerEnter(Collider other)
{
    if (other.gameObject.CompareTag("Dart"))
    {
        health--;

        if (health <= 0)
        {
            if (other.gameObject.GetComponent<BalloonGun>() != null)
            {
                gameManager.UpdateScore(50);
            }
            else
            {
                gameManager.UpdateScore(30);
            }

            Destroy(this.gameObject);
        }
    }
}

    private void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            Explode();
        }
    }
private void Explode()
{
    if (hasExploded) return;

    hasExploded = true;
    speed = 0;

    if (explosionPrefab != null)
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }

    MainTower tower = FindObjectOfType<MainTower>();
    if (tower != null)
    {
        tower.TakeDamage(explosionDamage);
    }

    Destroy(gameObject);
}

}


