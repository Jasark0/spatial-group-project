using UnityEngine;
using TMPro;

public class MainTower : MonoBehaviour
{
    public float health = 100f;
    public GameObject deathScreen;
    public TMP_Text scoreText;
    public TMP_Text[] healthText;
    public TextMeshPro towerHealthDisplay;
    public TextMeshPro playerScoreDisplay;
    private GameManager gameManager;

    public GameObject waves;


    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        UpdateHealth();
    } 

    void Update()
    {
        if (playerScoreDisplay != null)
        {
            playerScoreDisplay.text = "Score: " + gameManager.score;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        UpdateHealth();
        if (health <= 0)
        {
            Die();
            Destroy(gameObject); 
        }
    }

    private void Die()
    {
        // Get reference to player's camera/view
        Transform cameraTransform = null;
        if (Player.Instance != null)
        {
            // Try to find the center eye anchor (VR camera)
            cameraTransform = Player.Instance.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");
            
            if (cameraTransform == null)
            {
                // Fallback to camera rig if specific eye anchor isn't found
                cameraTransform = Player.Instance.transform.Find("OVRCameraRig");
            }
        }
        
        // Position and activate death screen in front of player
        if (cameraTransform != null)
        {
            // Get the forward direction but zero out the vertical component to keep it horizontal
            Vector3 horizontalForward = cameraTransform.forward;
            horizontalForward.y = 0;
            
            // If player is looking straight up/down, use the camera's up vector projected onto horizontal plane
            if (horizontalForward.magnitude < 0.1f)
            {
                horizontalForward = Vector3.ProjectOnPlane(cameraTransform.up, Vector3.up).normalized;
                // If still too small (edge case), use world forward
                if (horizontalForward.magnitude < 0.1f)
                {
                    horizontalForward = Vector3.forward;
                }
            }
            else
            {
                horizontalForward = horizontalForward.normalized;
            }
            
            // Position death screen at player's eye level, not too low
            float eyeHeight = cameraTransform.position.y;
            Vector3 position = cameraTransform.position + horizontalForward * 2f;
            
            // Ensure minimal height (never below player's feet)
            float minHeight = Player.Instance.transform.position.y + 1.0f; // 1 meter above player's feet
            position.y = Mathf.Max(eyeHeight - 0.3f, minHeight); // Slightly below eye level but not too low
            
            deathScreen.transform.position = position;
            
            // Make death screen face the player, but keep it upright
            Quaternion lookRotation = Quaternion.LookRotation(-horizontalForward, Vector3.up);
            // Apply 180-degree rotation around the Y-axis to make it face the player properly
            lookRotation *= Quaternion.Euler(0, 180, 0);
            deathScreen.transform.rotation = lookRotation;
        }
        
        // Store the original scale and set initial scale to zero
        // Vector3 originalScale = deathScreen.transform.localScale;
        // deathScreen.transform.localScale = Vector3.zero;
        
        // Activate the death screen
        deathScreen.SetActive(true);
        waves.SetActive(false);
        
        // Start animation coroutine to scale up
        // StartCoroutine(AnimateDeathScreen(originalScale));
        
        if (scoreText != null && gameManager != null)
        {
            scoreText.text = "Final Score: " + gameManager.score;
        }
        
        // Time.timeScale = 0f;
    }

    private System.Collections.IEnumerator AnimateDeathScreen(Vector3 targetScale)
    {
        float duration = 1.0f; // Animation duration in seconds
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            // Calculate progress (0 to 1)
            float t = elapsed / duration;
            
            // Apply easing for smoother animation (optional)
            t = Mathf.SmoothStep(0, 1, t);
            
            // Update scale
            deathScreen.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            
            // Wait for next frame
            elapsed += Time.unscaledDeltaTime; // Use unscaledDeltaTime because we set timeScale to 0
            yield return null;
        }
        
        // Ensure we end at exactly the target scale
        deathScreen.transform.localScale = targetScale;
    }

    private void UpdateHealth()
    {
        if (healthText != null)
        {
            foreach (TMP_Text text in healthText)
            {
                text.text = "Tower Health: " + health;
            }
        }
        
        if (towerHealthDisplay != null)
        {
            towerHealthDisplay.text = "HP: " + health.ToString("F0");
        }
    }
}
