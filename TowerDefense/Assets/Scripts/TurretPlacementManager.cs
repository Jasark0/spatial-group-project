using UnityEngine;
using UnityEngine.UI;
using System;

public class TurretPlacementManager : MonoBehaviour
{
    public GameObject[] turretPrefabs;
    private GameObject turretGhost;
    private GameObject selectedTurretPrefab;
    private bool isPlacing = false;
    private GameManager gameManager;

    public int[] turretCosts = { 200, 300 };

    private float currentZRotation = 0f;
    private int selectedTurretIndex = -1;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (isPlacing && turretGhost)
        {
            // Use the main camera's forward direction for raycasting
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                turretGhost.transform.position = hit.point;
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

        selectedTurretPrefab = turretPrefabs[turretIndex];
        turretGhost = Instantiate(selectedTurretPrefab);
        turretGhost.tag = "TurretGhost";
        turretGhost.GetComponent<Collider>().enabled = false;
        DisableTurretFunctionality(turretGhost);
        isPlacing = true;
        currentZRotation = 0f;
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
                isPlacing = false;
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
            isPlacing = false;
            Debug.Log("Turret placement canceled");
        }
    }
}
