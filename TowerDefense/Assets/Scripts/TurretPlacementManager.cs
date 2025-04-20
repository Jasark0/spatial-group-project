using UnityEngine;
using UnityEngine.UI;
using System;

public class TurretPlacementManager : MonoBehaviour
{
    public GameObject[] turretPrefabs;
    private GameObject turretGhost;
    private GameObject rangeIndicator;
    private GameObject selectedTurretPrefab;
    private bool isPlacing = false;
    private GameManager gameManager;
    [SerializeField] private Player player;

    public int[] turretCosts = { 200, 300, 500, 250 };

    private float currentZRotation = 0f;
    private int selectedTurretIndex = -1;
    public Material rangeMaterial;

    public Material rangeRestrictMaterial;

    [SerializeField] private LayerMask placementLayerMask;

    // Add this field to control how close turrets can be to each other
    [SerializeField] private float minTurretDistance = 1.5f; 
    
    // Track whether current position is valid
    [SerializeField] private bool isValidPlacement = true;

    [Header("Rotation Settings")]
    [SerializeField] [Range(50f, 200f)] private float rotationSensitivity = 120f;
    [SerializeField] [Range(0.05f, 0.5f)] private float thumbstickDeadzone = 0.1f;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>();
        
        // Subscribe to view mode change events
        if (player != null)
        {
            player.OnViewModeChanged += HandleViewModeChanged;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe when destroyed to prevent memory leaks
        if (player != null)
        {
            player.OnViewModeChanged -= HandleViewModeChanged;
        }
    }
    
    // Handler for view mode changes
    private void HandleViewModeChanged(Player.ViewMode newMode)
    {
        // If switching away from build mode while placing, cancel placement
        if (newMode != Player.ViewMode.Build && isPlacing)
        {
            CancelPlacement();
        }
    }

    void Update()
    {
        if (isPlacing && turretGhost)
        {
            // Use the main camera's forward direction for raycasting
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); 
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayerMask))
            {
                turretGhost.transform.position = hit.point;
                if (rangeIndicator != null)
                {
                    rangeIndicator.transform.position = hit.point;
                    
                    // Check if there are any turrets nearby that would prevent placement
                    CheckTurretProximity(hit.point);
                }
            }

            // Use Oculus controller trigger for placement
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                // Only allow placement if position is valid
                if (isValidPlacement)
                {
                    PlaceTurret();
                }
                else
                {
                    // Give feedback that placement is invalid
                    Debug.Log("Cannot place turret here — another turret is too close.");
                    // Optionally add haptic feedback here
                    StartCoroutine(FlashInvalidPlacement());
                }
            }

            // Use Oculus controller grip button to cancel
            if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger) )
            {
                CancelPlacement();
            }

            // Replace the keyboard-based rotation logic with joystick controls
            if (selectedTurretIndex == 3) // Barricade
            {
                // Get horizontal thumbstick input from left controller
                float rotationInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
                
                // Apply rotation based on thumbstick input with customizable sensitivity
                if (Mathf.Abs(rotationInput) > thumbstickDeadzone) // Use customizable deadzone
                {
                    // Use the public sensitivity value for rotation speed
                    float rotationSpeed = rotationSensitivity * Time.deltaTime; 
                    
                    // Apply rotation based on thumbstick direction
                    currentZRotation += rotationInput * rotationSpeed;
                    
                    // Keep rotation within 0-360 degrees
                    if (currentZRotation > 360f)
                        currentZRotation -= 360f;
                    else if (currentZRotation < 0f)
                        currentZRotation += 360f;
                        
                    // Apply the rotation to the ghost
                    Vector3 currentEuler = turretGhost.transform.rotation.eulerAngles;
                    turretGhost.transform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y, currentZRotation);
                }
            }
        }
    }

    public void SelectTurret(int turretIndex)
    {
        if (turretGhost) Destroy(turretGhost);
        if (rangeIndicator) Destroy(rangeIndicator);

        // Switch to build mode when selecting a turret
        player.SetViewMode(Player.ViewMode.Build);

        selectedTurretPrefab = turretPrefabs[turretIndex];
        turretGhost = Instantiate(selectedTurretPrefab);
        turretGhost.tag = "TurretGhost";
        turretGhost.GetComponent<Collider>().enabled = false;
        
        // Create range indicator
        if (turretGhost.GetComponent<Turret>() != null)
        {
            CreateRangeIndicator(turretGhost.GetComponent<Turret>().range);
        }
        
        DisableTurretFunctionality(turretGhost);
        isPlacing = true;
        currentZRotation = 0f;
        selectedTurretIndex = turretIndex;
    }

    private void CreateRangeIndicator(float range)
    {
        // Create a sphere to visualize the range
        rangeIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rangeIndicator.transform.localScale = new Vector3(range * 2, range * 2, range * 2);
        
        // Make it semi-transparent
        Renderer renderer = rangeIndicator.GetComponent<Renderer>();
        renderer.material = rangeMaterial;
        
        // Remove the collider so it doesn't interfere with raycasting
        Destroy(rangeIndicator.GetComponent<Collider>());
        
        // Start with valid placement state
        isValidPlacement = true;
    }

    void PlaceTurret()
    {
        if (turretGhost)
        {
            int cost = turretCosts[Array.IndexOf(turretPrefabs, selectedTurretPrefab)];

            // We already check proximity in the Update method, so no need for this check here
            // This keeps the code clean and avoids duplication
            
            if (gameManager.CanAfford(cost))
            {
                Instantiate(selectedTurretPrefab, turretGhost.transform.position, turretGhost.transform.rotation);
                gameManager.DeductMoney(cost);
                Destroy(turretGhost);
                Destroy(rangeIndicator);
                isPlacing = false;
                
                // Switch back to first-person view when placement is successful
                player.SetViewMode(Player.ViewMode.FirstPerson);
            }
            else
            {
                Debug.Log("Not enough money to place turret");
            }
        }
    }

    void DisableTurretFunctionality(GameObject turret)
    {
        MonoBehaviour[] scripts = turret.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (!(script is TurretPlacementManager))
            {
                script.enabled = false;
            }
        }
    }
    
    void CancelPlacement()
    {
        if (turretGhost)
        {
            Destroy(turretGhost);
            Destroy(rangeIndicator);
            isPlacing = false;
            Debug.Log("Turret placement canceled");
            
            // Switch back to first-person view when canceling placement
            player.SetViewMode(Player.ViewMode.FirstPerson);
        }
    }
    
    private void CheckTurretProximity(Vector3 position)
    {
        // Find all turrets in the scene (including ghosts)
        Collider[] colliders = Physics.OverlapSphere(position, minTurretDistance);
        
        bool tooClose = false;
        foreach (var col in colliders)
        {
            if (col.gameObject.CompareTag("Turret"))
            {
                tooClose = true;
                break;
            }
        }
        
        // Update material based on placement validity
        if (tooClose != !isValidPlacement) // Only update if state changed
        {
            isValidPlacement = !tooClose;
            
            Renderer renderer = rangeIndicator.GetComponent<Renderer>();
            if (isValidPlacement)
            {
                renderer.material = rangeMaterial;
            }
            else
            {
                renderer.material = rangeRestrictMaterial;
            }
        }
    }
    
    // Visual feedback for invalid placement
    private System.Collections.IEnumerator FlashInvalidPlacement()
    {
        if (rangeIndicator == null) yield break;
        
        Renderer renderer = rangeIndicator.GetComponent<Renderer>();
        Color originalColor = rangeRestrictMaterial.color;
        
        // Store original alpha
        float originalAlpha = originalColor.a;
        
        // Flash by increasing alpha
        Color flashColor = originalColor;
        flashColor.a = Mathf.Min(0.8f, originalAlpha * 1.5f);
        rangeRestrictMaterial.color = flashColor;
        
        yield return new WaitForSeconds(0.2f);
        
        // Return to original alpha
        rangeRestrictMaterial.color = originalColor;
    }
}
