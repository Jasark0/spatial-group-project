using UnityEngine;
using TMPro;
using System.Collections;

public class TurretHealthDisplay : MonoBehaviour
{
    private TextMeshPro healthText;
    private Turret turret;
    private Camera mainCamera;

    // Max health is tracked when the display starts
    private float maxHealth;
    public float maxViewDistance = 30f;

    void Start()
    {
        // Get the TMP component
        healthText = GetComponent<TextMeshPro>();

        // Get the parent turret component
        turret = GetComponentInParent<Turret>();

        // Store the initial health as max health
        if (turret)
        {
            maxHealth = turret.health;
        }

        // Get the main camera
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!turret || !healthText || !mainCamera) return;

        // Calculate health percentage
        float healthPercentage = turret.health / maxHealth;

        // Update health text
        healthText.text = $"{Mathf.Ceil(turret.health)}";

        // Set text color based on health percentage
        if (healthPercentage > 0.6f)
            healthText.color = Color.green;
        else if (healthPercentage > 0.3f)
            healthText.color = Color.yellow;
        else
            healthText.color = Color.red;

        // Make text face the camera
        transform.rotation = mainCamera.transform.rotation;

        // Scale text based on distance to camera
        float distanceToCamera = Vector3.Distance(mainCamera.transform.position, transform.position);

        // Optional: Fade text based on distance (if too far)
        if (distanceToCamera > maxViewDistance * 0.9f)
        {
            float alpha = Mathf.Clamp01(1.0f - (distanceToCamera - maxViewDistance * 0.9f) / (maxViewDistance * 0.1f));
            Color textColor = healthText.color;
            textColor.a = alpha;
            healthText.color = textColor;
        }
    }

    // Helper function to add shake effect when turret takes damage
    public void ShakeText(float intensity = 0.2f, float duration = 0.3f)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = originalPosition.x + Random.Range(-1f, 1f) * intensity;
            float y = originalPosition.y + Random.Range(-1f, 1f) * intensity;

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}