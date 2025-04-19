using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretUpgradeRadialMenu : MonoBehaviour
{

    [Header("References")]
    public RadialSelection radialSelection;
    public GameObject turretNameDisplay;
    public GameObject turretHealthDisplay;
    public GameObject turretCostDisplay;
    public GameObject turretLevelDisplay;

    [Header("Ray Settings")]
    public Transform rayOrigin;
    public float maxRayDistance = 30f;
    public LayerMask turretLayerMask;

    public bool ifTurretIsSelected;

    [Header("nah")]
    private Turret selectedTurret;
    private Turret currentlySelecting;
    private GameManager gameManager;
    private const int FIRE_RATE_OPTION = 0;
    private const int HEALTH_OPTION = 1;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        radialSelection.OnPartSelected.AddListener(HandleRadialSelection);

    }

    private void Update()
    {
        RaycastDetection();
        if (radialSelection.spawnedParts.Count > 0)
        {
            UpdateText();
        }
    }
    
    private void RaycastDetection()
    {
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, maxRayDistance, turretLayerMask))
        {
            Turret turret = hit.collider.GetComponent<Turret>();
            if (turret != null)
            {
                currentlySelecting = turret;
                selectedTurret = turret;
                // Allow opening the radial menu with controller button
                // RadialSelection will handle its own button detection
            }

            ifTurretIsSelected = true;
        }
        else
        {
            // Hide the display when not looking at a turret
            /* if (turretNameDisplay)
                turretNameDisplay.SetActive(false); */
                
            currentlySelecting = null;
            ifTurretIsSelected = false;
        }
    }
    
    public void HandleRadialSelection(int optionIndex)
    {
        if (selectedTurret == null) return;
        
        int upgradeCost = 100; // Get this from your game settings
        
        switch (optionIndex)
        {
            case FIRE_RATE_OPTION:
                if (gameManager.CanAfford(upgradeCost))
                {
                    gameManager.DeductMoney(upgradeCost);
                    selectedTurret.UpgradeFireRate();
                    Debug.Log("Fire Rate Upgrade");
                }
                break;
                
            case HEALTH_OPTION:
                if (gameManager.CanAfford(upgradeCost))
                {
                    gameManager.DeductMoney(upgradeCost);
                    selectedTurret.UpgradeHealth();
                    Debug.Log("Health Upgrade");
                }
                break;
        }
    }

    public void UpdateRadialSelectionText(int optionIndex)
    {
        switch (optionIndex)
        {
            case FIRE_RATE_OPTION:
                turretLevelDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = $"Fire Rate lvl: {selectedTurret.fireRateLevel} --> {selectedTurret.fireRateLevel + 1}";
                turretCostDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = $"Cost: {100}"; // Get this from your game settings
                turretLevelDisplay.gameObject.SetActive(true);
                turretCostDisplay.gameObject.SetActive(true);

                break;
                
            case HEALTH_OPTION:
                turretLevelDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = $"Health lvl: {selectedTurret.healthLevel} --> {selectedTurret.healthLevel + 1}";
                turretCostDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = $"Cost: {100}"; // Get this from your game settings
                turretLevelDisplay.gameObject.SetActive(true);
                turretCostDisplay.gameObject.SetActive(true);
                break;
                
            default:
                turretLevelDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "";
                turretCostDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "";
                break;
        }
    }

    public void UpdateText()
    {
        if (selectedTurret == null) return;

        turretNameDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = selectedTurret.gameObject.name;
        turretHealthDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = $"Health: {selectedTurret.health}";
        
        for ( int i = 0; i < radialSelection.spawnedParts.Count; i++)
        {
            if (i == FIRE_RATE_OPTION)
            {
                radialSelection.spawnedParts[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"Fire Rate\nUpgrade";
                // turretCostDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = $"Cost: {100}"; // Get this from your game settings

            }
            else if (i == HEALTH_OPTION)
            {
                radialSelection.spawnedParts[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"Health level\nUpgrade";
                // turretCostDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = $"Cost: {100}"; // Get this from your game settings
            }
            else
            {
                radialSelection.spawnedParts[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Exit";
            }
        }
    }
    



}
