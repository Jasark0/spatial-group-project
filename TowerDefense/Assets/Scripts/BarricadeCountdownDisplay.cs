using UnityEngine;
using TMPro;
using System.Collections;

// Manages the UI or visual countdown timer related to barricades,
// shows players how long until a barricade becomes active or disappears.

public class BarricadeCountdownDisplay : MonoBehaviour
{
    private TextMeshPro countdownText;
    private Barricade barricade;
    private Camera mainCamera;

    public float maxViewDistance = 30f;

    void Start()
    {
        // Get the TMP component
        countdownText = GetComponent<TextMeshPro>();

        // Get the parent barricade component
        barricade = GetComponentInParent<Barricade>();

        // Get the main camera
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!barricade || !countdownText || !mainCamera) return;

        // Get remaining lifetime
        float remainingTime = barricade.GetRemainingLifetime();

        // Update countdown text - show seconds with one decimal place
        countdownText.text = $"{remainingTime:F1}s";

        // Set text color based on remaining time percentage
        float timePercentage = remainingTime / barricade.lifetime;
        
        if (timePercentage > 0.6f)
            countdownText.color = Color.green;
        else if (timePercentage > 0.3f)
            countdownText.color = Color.yellow;
        else
            countdownText.color = Color.red;

        // Make text face the camera
        transform.rotation = mainCamera.transform.rotation;

        // Optional: Fade text based on distance (if too far)
        float distanceToCamera = Vector3.Distance(mainCamera.transform.position, transform.position);
        if (distanceToCamera > maxViewDistance * 0.9f)
        {
            float alpha = Mathf.Clamp01(1.0f - (distanceToCamera - maxViewDistance * 0.9f) / (maxViewDistance * 0.1f));
            Color textColor = countdownText.color;
            textColor.a = alpha;
            countdownText.color = textColor;
        }
    }
    
    // Add a pulsing effect when countdown is near the end
    public void PulseText()
    {
        StartCoroutine(PulseCoroutine());
    }
    
    private IEnumerator PulseCoroutine()
    {
        Vector3 originalScale = transform.localScale;
        float pulseDuration = 0.5f;
        float elapsed = 0f;
        float pulseSize = 1.2f;
        
        while (elapsed < pulseDuration)
        {
            float t = elapsed / pulseDuration;
            float scale = Mathf.Lerp(1f, pulseSize, Mathf.Sin(t * Mathf.PI));
            transform.localScale = originalScale * scale;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
} 