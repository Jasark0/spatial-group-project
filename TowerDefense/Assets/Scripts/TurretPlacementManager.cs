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
    private Player player;

    public int[] turretCosts = { 200, 300 };

    private float currentZRotation = 0f;
    private int selectedTurretIndex = -1;
    public Material rangeMaterial;

    [SerializeField] private LayerMask placementLayerMask;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>();
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
                }
            }

            // Use Oculus controller trigger for placement
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                PlaceTurret();
            }

            // Use Oculus controller grip button to cancel
            if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger))
            {
                CancelPlacement();
            }

            if (selectedTurretIndex == 3 && Input.GetKeyDown(KeyCode.R))
            {
                currentZRotation += 45f;
                if (currentZRotation > 360f)
                    currentZRotation = 45f;

                Vector3 currentEuler = turretGhost.transform.rotation.eulerAngles;
                turretGhost.transform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y, currentZRotation);
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
        CreateRangeIndicator(turretGhost.GetComponent<Turret>().range);
        
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
        // rangeMaterial.color = rangeColor;
        renderer.material = rangeMaterial;
        
        // Remove the collider so it doesn't interfere with raycasting
        Destroy(rangeIndicator.GetComponent<Collider>());
    }

    void PlaceTurret()
    {
        if (turretGhost)
        {
            int cost = turretCosts[Array.IndexOf(turretPrefabs, selectedTurretPrefab)];
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
}
