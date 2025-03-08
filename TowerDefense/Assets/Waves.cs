using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public float difficulty = 0.4f;
    public float difficultyIncreaseSpeed = 0.01f;
    public GameObject balloonGreen;
    public GameObject balloonRed;

    // Timer
    private float balloonTimer = 0f;
    private float nextBalloon = 2f;

    // Balloon limiter
    public int balloonsPerWave = 20;
    private int balloonsCount = 1;

    // Waves timer
    public float wavesTimer = 0f;
    public float nextWave = 20f;

    private GameManager gameManager;

    // Plane dimensions
    public Transform planeTransform;
    private float planeSizeX;
    private float planeSizeZ;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (planeTransform != null)
        {
            Renderer planeRenderer = planeTransform.GetComponent<Renderer>();
            if (planeRenderer != null)
            {
                planeSizeX = planeRenderer.bounds.size.x / 2f;
                planeSizeZ = planeRenderer.bounds.size.z / 2f;
            }
        }
    }

    void Update()
    {
        if (balloonTimer < Time.time && wavesTimer < Time.time)
        {
            balloonsCount++;
            difficulty += difficultyIncreaseSpeed;
            balloonTimer = Time.time + nextBalloon;

            float chance = Random.Range(10f, 20f);
            GameObject balloonToSpawn = (Random.Range(0f, 100f) < chance) ? balloonRed : balloonGreen;

            Vector3 spawnPosition = GetRandomSpawnPosition();
            var balloon = Instantiate(balloonToSpawn, spawnPosition, balloonToSpawn.transform.rotation);

            balloon.GetComponent<Balloon>().health += (int)Mathf.Round(difficulty);
            balloon.GetComponent<Balloon>().speed += (int)Mathf.Round(difficulty);
        }

        if (balloonsCount % balloonsPerWave == 0 && wavesTimer < Time.time)
        {
            wavesTimer = nextWave + Time.time;
            gameManager.UpdateMoney(100);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        int edge = Random.Range(0, 4);
        float x = 0, z = 0;

        switch (edge)
        {
            case 0:
                x = Random.Range(-planeSizeX, planeSizeX);
                z = planeSizeZ;
                break;
            case 1:
                x = Random.Range(-planeSizeX, planeSizeX);
                z = -planeSizeZ;
                break;
            case 2:
                x = -planeSizeX;
                z = Random.Range(-planeSizeZ, planeSizeZ);
                break;
            case 3:
                x = planeSizeX;
                z = Random.Range(-planeSizeZ, planeSizeZ);
                break;
        }

        return new Vector3(x, 2, z);
    }
}
