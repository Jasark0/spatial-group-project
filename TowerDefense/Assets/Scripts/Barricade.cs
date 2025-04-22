using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour
{
    public float lifetime = 30f;
    private float remainingLifetime;
    
    public Vector3 canvasOffset = new Vector3(0, 2, 0);
    
    private BarricadeCountdownDisplay countdownDisplay;
    private bool pulseTrigger10s = false;
    private bool pulseTrigger5s = false;

    private void Start()
    {
        remainingLifetime = lifetime;
        Destroy(gameObject, lifetime);
        
        // Find the countdown display component if it exists
        countdownDisplay = GetComponentInChildren<BarricadeCountdownDisplay>();
    }
    
    private void Update()
    {
        remainingLifetime -= Time.deltaTime;
        
        // Trigger pulsing effect at certain thresholds
        if (countdownDisplay != null)
        {
            if (remainingLifetime <= 10f && !pulseTrigger10s)
            {
                countdownDisplay.PulseText();
                pulseTrigger10s = true;
            }
            
            if (remainingLifetime <= 5f && !pulseTrigger5s)
            {
                countdownDisplay.PulseText();
                pulseTrigger5s = true;
            }
            
            // Pulse every second when under 5 seconds
            if (remainingLifetime <= 5f && Mathf.Floor(remainingLifetime) != Mathf.Floor(remainingLifetime + Time.deltaTime))
            {
                countdownDisplay.PulseText();
            }
        }
    }
    
    public float GetRemainingLifetime()
    {
        return remainingLifetime;
    }
}
