using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public float difficulty = 0.4f;
    public float difficultyIncreaseSpeed = 0.01f;
    public Transform startPosition;
    public GameObject balloonGreen;

    // Timer
    private float balloonTimer = 0f;
    private float nextBalloon = 1f;

    //Balloon limiter
    public int balloonsPerWave = 20;
    private int balloonsCount = 1;
    // Waves timer
    public float wavesTimer = 0f;
    public float nextWave = 20f;

    // Update is called once per frame
    void Update()
    {
        // Send Balloons
        if (balloonTimer < Time.time && wavesTimer < Time.time)
        {
            balloonsCount++;
            difficulty += difficultyIncreaseSpeed;
            balloonTimer = Time.time + nextBalloon;

            var balloon = Instantiate(balloonGreen, startPosition.position + new Vector3(0, 2, 0), balloonGreen.transform.rotation);
            balloon.GetComponent<Balloon>().health += (int)System.Math.Round(difficulty);
            balloon.GetComponent<Balloon>().speed += (int)System.Math.Round(difficulty);
        }

        // Create waves
        if ( balloonsCount % balloonsPerWave == 0 && wavesTimer < Time.time)
        {
            wavesTimer = nextWave + Time.time;
        }
        
    }
}
