using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public float difficulty = 0.4f;
    public float difficultyIncreaseSpeed = 0.01f;
    public Transform startPosition;
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

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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

            var balloon = Instantiate(balloonToSpawn, startPosition.position + new Vector3(0, 2, 0), balloonToSpawn.transform.rotation);
            balloon.GetComponent<Balloon>().health += (int)System.Math.Round(difficulty);
            balloon.GetComponent<Balloon>().speed += (int)System.Math.Round(difficulty);
        }

        if (balloonsCount % balloonsPerWave == 0 && wavesTimer < Time.time)
        {
            wavesTimer = nextWave + Time.time;
            gameManager.UpdateMoney(100);
        }
    }
}
