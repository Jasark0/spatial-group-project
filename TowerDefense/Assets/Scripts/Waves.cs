using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Waves : MonoBehaviour
{
    public float difficulty = 0.4f;
    public float difficultyIncreaseSpeed = 0.01f;
    public GameObject balloonGreen;
    public GameObject balloonRed;
    public GameObject balloonGreenLarge;

    // Timer
    private float balloonTimer = 0f;
    private float nextBalloon = 3f;
    private float minBalloonSpawnTime = 0.8f;

    // Wave system
    public int balloonsPerWave = 20;
    private int balloonsCount = 0;
    private int currentWave = 1;

    public float wavesTimer = 0f;
    public float nextWave = 20f;

    private GameManager gameManager;

    // Plane dimensions
    public Transform planeTransform;
    private float planeSizeX;
    private float planeSizeZ;

    // UI
    public TMP_Text waveText;

    // Red balloon chance
    private float redBalloonChance = 10f;
    private float maxRedBalloonChance = 30f;

    // Green Large Balloon chance
    private float greenLargeBalloonChance = 0f;
    private float maxGreenLargeBalloonChance = 30f;

    // Money increase
    private int baseMoneyReward = 200;
    private int moneyIncreaseInterval = 5;

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

        UpdateWaveUI();
    }

    void Update()
    {
        if (balloonTimer < Time.time && wavesTimer < Time.time)
        {
            balloonsCount++;
            difficulty += difficultyIncreaseSpeed;
            balloonTimer = Time.time + nextBalloon;
            
            float redChance = Mathf.Min(redBalloonChance + (currentWave * 2), maxRedBalloonChance);

            if (currentWave > 5)
            {
                greenLargeBalloonChance = Mathf.Min((currentWave - 5) * 5, maxGreenLargeBalloonChance);
            }

            float randomValue = Random.Range(0f, 100f);
            GameObject balloonToSpawn;

            if (randomValue < greenLargeBalloonChance)
            {
                balloonToSpawn = balloonGreenLarge;
            }
            else if (randomValue < redChance)
            {
                balloonToSpawn = balloonRed;
            }
            else
            {
                balloonToSpawn = balloonGreen;
            }

            Vector3 spawnPosition = GetRandomSpawnPosition();
            var balloon = Instantiate(balloonToSpawn, spawnPosition, balloonToSpawn.transform.rotation);

            balloon.GetComponent<Balloon>().health += (int)Mathf.Round(difficulty);
            balloon.GetComponent<Balloon>().speed += (int)Mathf.Round(difficulty);
        }

        if (balloonsCount >= balloonsPerWave)
        {
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        balloonsCount = 0;
        currentWave++;
        wavesTimer = nextWave + Time.time;

        if (currentWave % moneyIncreaseInterval == 0)
        {
            baseMoneyReward += 200;
        }

        gameManager.UpdateMoney(baseMoneyReward);
        gameManager.UpdateScore(100);
        nextBalloon = Mathf.Max(3f - (currentWave * 0.2f), minBalloonSpawnTime);

        UpdateWaveUI();
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

    private void UpdateWaveUI()
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + currentWave;
        }
    }
}
