using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class GrabbableGun : MonoBehaviour
{
    private Grabbable grabbable;
    public GameObject dartPrefab;
    public Transform barrelLocation;

    public bool inHolster = false;

    public MeshFilter gunMeshFilter;

    public float shotPower = 1000f;
    public float fireRate = 0.6f;
    public float arFireRate = 0.3f;
    private float nextFireTime = 0f;

    [Header("Gun Type")]
    public bool isPistol;
    public bool isSMG;
    public bool isAR;
    private bool isFiring = false;
    
    // Reference to check if right controller is holding anything
    private Oculus.Interaction.GrabInteractor rightGrabInteractor;

    void Start()
    {
        grabbable = GetComponent<Grabbable>();

        if (gunMeshFilter == null)
        {
            gunMeshFilter = GetComponent<MeshFilter>();
        }
   
    }
    


    void Update()
    {
        if (grabbable.SelectingPointsCount > 0)
        {
            OVRInput.Controller controller = DetermineHoldingController();

            if (Player.instance.IsInBuildMode())
                return;

            if ( inHolster)
                {
                    return;
                }

            // Pistol (semi-auto)
            if (isPistol && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller) && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }

            // SMG (burst)
            else if (isSMG && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller) && Time.time >= nextFireTime)
            {
                StartCoroutine(BurstFire(4));
                nextFireTime = Time.time + fireRate;
            }

            // AR (full auto)
            else if (isAR && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + arFireRate;
            }
        }
    }

        IEnumerator BurstFire(int burstCount)
    {
        for (int i = 0; i < burstCount; i++)
        {
            Shoot();
            yield return new WaitForSeconds(0.08f);
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
}
