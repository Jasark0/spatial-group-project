using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class GrabbableGun : MonoBehaviour
{
    private Grabbable grabbable;
    public GameObject dartPrefab;
    public Transform barrelLocation;

    public float shotPower = 1000f;

    void Start()
    {
        grabbable = GetComponent<Grabbable>();
    }

    void Update()
    {
        // Check if player is pulling the trigger
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) && grabbable.SelectingPointsCount > 0)
        {
            Shoot();
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
}
