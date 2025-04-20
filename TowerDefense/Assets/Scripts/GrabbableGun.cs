using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class GrabbableGun : MonoBehaviour
{
    private Grabbable grabbable;
    public GameObject dartPrefab;
    public Transform barrelLocation;

    public MeshFilter gunMeshFilter;
    public Mesh pistolMesh;
    public Mesh smgMesh;

    public float shotPower = 1000f;
    public float fireRate = 0.6f;
    private float nextFireTime = 0f;
    public bool isUpgraded = false;
    private int upgradeCost = 500;
    private GameManager gameManager;
    
    // Reference to check if right controller is holding anything
    private Oculus.Interaction.GrabInteractor rightGrabInteractor;

    void Start()
    {
        grabbable = GetComponent<Grabbable>();
        gameManager = FindObjectOfType<GameManager>();

        if (gunMeshFilter == null)
        {
            gunMeshFilter = GetComponent<MeshFilter>();
        }

        if (gunMeshFilter != null && pistolMesh != null)
        {
            gunMeshFilter.mesh = pistolMesh;
        }
        
   
    }
    


    void Update()
    {
        // Only proceed if the gun is being grabbed
        if (grabbable.SelectingPointsCount > 0)
        {
           
            // Determine which controller is holding the gun
            OVRInput.Controller holdingController = DetermineHoldingController();
            
            // Check for trigger press on the appropriate controller
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, holdingController) && 
                Time.time >= nextFireTime)
            {
                if (Player.instance.IsInBuildMode())
                {
                    return;
                }
                Shoot();
                nextFireTime = Time.time + fireRate;
            }

            
        } 
     
    }

    private OVRInput.Controller DetermineHoldingController()
    {
        // Check which hand is grabbing the gun
        if (grabbable.GrabPoints.Count > 0)
        {
            // Get position of the grab point relative to the player
            Vector3 grabPos = grabbable.GrabPoints[0].position;
            Transform cameraTransform = Camera.main.transform;
            
            // If grab position is on the right side of the camera, it's likely the right hand
            if (Vector3.Dot(cameraTransform.right, grabPos - cameraTransform.position) > 0)
            {
                return OVRInput.Controller.RTouch;
            }
            else
            {
                return OVRInput.Controller.LTouch;
            }
        }
        
        // Default to both controllers if we can't determine
        return OVRInput.Controller.Touch;
    }

    void Shoot()
    {
        // Instantsiate a new dart at the barrel location in the barrellocation rotation
        var dart = Instantiate(dartPrefab, barrelLocation.position, barrelLocation.transform.rotation);
        if (dart.TryGetComponent<Bullet>(out var bulletScript))
        {
            bulletScript.Init(barrelLocation.forward, shotPower, "Turret");
        }
        // Desetroy the dart after X seconds.
        Destroy(dart, 3f);
    }

    void TryUpgradeGun()
    {
        if (gameManager != null && gameManager.CanAfford(upgradeCost))
        {
            gameManager.DeductMoney(upgradeCost);

            fireRate = 0.2f;
            isUpgraded = true;

            if (gunMeshFilter != null && smgMesh != null)
            {
                gunMeshFilter.mesh = smgMesh;
            }

            Debug.Log("Gun upgraded to SMG!");
        }
        else
        {
            Debug.Log("Not enough money to upgrade.");
        }
    }
}
