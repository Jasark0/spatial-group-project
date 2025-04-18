using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float health = 10;
    public float speed = 0.5f;
    private GameManager gameManager;
    public Animator animator;
    private bool hasExploded = false;
    public GameObject explosionPrefab;
    public float explosionDamage = 5f;
    private Transform target;
    public float minYThreshold = -100f; 

    private GameObject flashObject;

    protected virtual void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        flashObject = this.transform.Find("Flash").gameObject;
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

        if (transform.position.y < minYThreshold)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        Flash();
        health -= amount;
        if (health <= 0f)
        {
            gameManager.UpdateScore(30);
            Explode();
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

        if (Vector3.Distance(transform.position, target.position) < 7f)
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

    private void Flash()
    {

        if (flashObject == null)
        {
            Debug.LogError("Flash object not found!");
            return;
        }
        flashObject.SetActive(true);
        Invoke("HideFlash", 0.1f);
        // flashObject.SetActive(false);
    }

    private void HideFlash()
    {
        flashObject.SetActive(false);
    }

}


