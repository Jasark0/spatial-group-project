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
    public GameObject flyingBalloon;

    // Timer
    private float balloonTimer = 0f;
    private float nextBalloon = 3f;
    private float minBalloonSpawnTime = 0.8f;
    private float startDelayTimer = 0f;
    private bool gameStarted = false;

    // Wave system
    public int balloonsPerWave;
    private int balloonsCount = 0;
    protected int currentWave = 1;
    private bool waitingForNextWave = false;
    private float waveCooldownTimer = 0f;

    protected GameManager gameManager;

    // Plane dimensions
    public Transform planeTransform;
    protected float planeSizeX;
    protected float planeSizeZ;

    // UI
    public TMP_Text[] waveText;

    // Red balloon chance
    private float redBalloonChance = 10f;
    private float maxRedBalloonChance = 30f;

    // Green Large Balloon chance
    private float greenLargeBalloonChance = 0f;
    private float maxGreenLargeBalloonChance = 30f;

    // Flying Balloon chance
    private float flyingBalloonChance = 0f;
    private float maxFlyingBalloonChance = 25f;

    // Money increase
    private int baseMoneyReward = 250;
    private int moneyIncreaseInterval = 5;

    protected virtual void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        startDelayTimer = Time.time + 10f;
        if (planeTransform != null)
        {
            Renderer planeRenderer = planeTransform.GetComponent<Renderer>();
            if (planeRenderer != null)
            {
                planeSizeX = planeRenderer.bounds.size.x / 2f;
                planeSizeZ = planeRenderer.bounds.size.z / 2f;
            }
        }

        balloonsPerWave = 3;
        UpdateWaveUI();
    }

    protected virtual void Update()
    {
        if (!gameStarted)
        {
            if (Time.time >= startDelayTimer)
            {
                gameStarted = true;
                PopUpManager.Instance.ShowPopUp(0, 5, "Wave " + currentWave + " Starting!");
            }
            else
            {
                return;
            }
        }

        if (balloonTimer < Time.time && !waitingForNextWave)
        {
            if (balloonsCount < balloonsPerWave)
            {
                SpawnBalloon();
            }
            else
            {
                GameObject[] activeBalloons = GameObject.FindGameObjectsWithTag("Enemy");

                if (activeBalloons.Length == 0)
                {
                    waitingForNextWave = true;
                    PopUpManager.Instance.ShowPopUp(0, 5, "Wave " + currentWave + " Complete!");
                    waveCooldownTimer = Time.time + 15f;
                }
            }
        }

        if (waitingForNextWave && Time.time >= waveCooldownTimer)
        {
            StartNextWave();
            PopUpManager.Instance.ShowPopUp(0, 5, "Wave " + currentWave + " Starting!");
            waitingForNextWave = false;
        }
    }

    private void SpawnBalloon()
    {
        balloonsCount++;
        difficulty += difficultyIncreaseSpeed;
        balloonTimer = Time.time + nextBalloon;

        float redChance = Mathf.Min(redBalloonChance + (currentWave * 2), maxRedBalloonChance);

        if (currentWave > 5)
        {
            greenLargeBalloonChance = Mathf.Min((currentWave - 5) * 5, maxGreenLargeBalloonChance);
        }

        if (currentWave > 10)
        {
            flyingBalloonChance = Mathf.Min((currentWave - 10) * 3, maxFlyingBalloonChance);
        }

        float randomValue = Random.Range(0f, 100f);
        GameObject balloonToSpawn;

        if (randomValue < flyingBalloonChance)
        {
            balloonToSpawn = flyingBalloon;
        }
        else if (randomValue < greenLargeBalloonChance)
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

        Vector3 spawnPosition = GetRandomSpawnPosition(balloonToSpawn);
        var balloon = Instantiate(balloonToSpawn, spawnPosition, balloonToSpawn.transform.rotation);

        balloon.GetComponent<Balloon>().health += (int)Mathf.Round(difficulty);
        balloon.GetComponent<Balloon>().speed += (int)Mathf.Round(difficulty);
    }

    protected virtual void StartNextWave()
    {
        balloonsCount = 0;
        currentWave++;

        if (currentWave <= 10)
            balloonsPerWave = 3 * currentWave;
        else
            balloonsPerWave += 1;

        if (currentWave % moneyIncreaseInterval == 0)
        {
            baseMoneyReward += 200;
        }

        gameManager.UpdateMoney(baseMoneyReward);
        gameManager.UpdateScore(100);
        nextBalloon = Mathf.Max(3f - (currentWave * 0.2f), minBalloonSpawnTime);

        UpdateWaveUI();
    }

    protected virtual Vector3 GetRandomSpawnPosition(GameObject balloon)
    {
        int edge = Random.Range(0, 4);
        float x = 0, z = 0, y = 0;
        Vector3 planeCenter = planeTransform.position;

        switch (edge)
        {
            case 0: // Top edge
                x = Random.Range(planeCenter.x - planeSizeX, planeCenter.x + planeSizeX);
                z = planeCenter.z + planeSizeZ;
                break;
            case 1: // Bottom edge
                x = Random.Range(planeCenter.x - planeSizeX, planeCenter.x + planeSizeX);
                z = planeCenter.z - planeSizeZ;
                break;
            case 2: // Left edge
                x = planeCenter.x - planeSizeX;
                z = Random.Range(planeCenter.z - planeSizeZ, planeCenter.z + planeSizeZ);
                break;
            case 3: // Right edge
                x = planeCenter.x + planeSizeX;
                z = Random.Range(planeCenter.z - planeSizeZ, planeCenter.z + planeSizeZ);
                break;
        }

        if (balloon == flyingBalloon)
        {
            y = Random.Range(15f, 30f);
        }

        return new Vector3(x, y, z);
    }

    private void UpdateWaveUI()
    {
        if (waveText != null)
        {
            foreach (TMP_Text waveText in waveText)
            {
                waveText.text = "Wave: " + currentWave;
            }
        }
    }
}

