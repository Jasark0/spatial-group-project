
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holster : MonoBehaviour
{
    public GameObject centerEyeAnchor;
    private float rotationSpeed = 50;

    public GameObject[] holsters;

    private void Update()
    {
        // Put holdster halfway between the body
        transform.position = new Vector3(centerEyeAnchor.transform.position.x, centerEyeAnchor.transform.position.y, centerEyeAnchor.transform.position.z);

        var rotationDifference = Math.Abs(centerEyeAnchor.transform.eulerAngles.y - transform.eulerAngles.y);
        var finalRotationSpeed = rotationSpeed;

        // Make rotation speed faster if holster rotation is further away from the central eye camera
        if ( rotationDifference > 60)
        {
            finalRotationSpeed = rotationSpeed * 2;
        }
        else if (rotationDifference > 40 && rotationDifference < 60)
        {
            finalRotationSpeed = rotationSpeed;
        }
        else if ( rotationDifference < 40 && rotationDifference > 20)
        {
            finalRotationSpeed = rotationSpeed /2;
        }
        else if ( rotationDifference < 20 && rotationDifference > 0)
        {
            finalRotationSpeed = rotationSpeed / 4;
        }

        var step = finalRotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, centerEyeAnchor.transform.eulerAngles.y, 0), step);

    }


}
