using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartGun : MonoBehaviour
{
    public GameObject dartPrefab;
    public Transform barrelLocation;

    public float shotPower = 1000f;

    void Update()
    {
        // Check if player is pulling the trigger
        if ( OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Instantsiate a new dart at the barrel location in the barrellocation rotation
        var dart = Instantiate(dartPrefab, barrelLocation.position, barrelLocation.transform.rotation);
        // Add force to the Dart rigidbody component
        dart.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        // Spin the dart in the correct direction. if this is wrong for you try other values until it's correct
        // dart.transform.eulerAngles += new Vector3(0, -90, 0);
        dart.transform.rotation *= Quaternion.Euler(0, -90, -90);
        // Desetroy the dart after X seconds.
        Destroy(dart, 3f);
    }
}
