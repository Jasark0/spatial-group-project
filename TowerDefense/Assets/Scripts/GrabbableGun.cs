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
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) &&
            grabbable.SelectingPointsCount > 0)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }

        if (!isUpgraded && OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) &&
            grabbable.SelectingPointsCount > 0)
        {
            TryUpgradeGun();
        }
    }

    void Shoot()
    {
        // Instantsiate a new dart at the barrel location in the barrellocation rotation
        var dart = Instantiate(dartPrefab, barrelLocation.position, barrelLocation.transform.rotation);
        // Spin the dart in the correct direction. if this is wrong for you try other values until it's correct
        // dart.transform.eulerAngles += new Vector3(0, -90, 0);
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
