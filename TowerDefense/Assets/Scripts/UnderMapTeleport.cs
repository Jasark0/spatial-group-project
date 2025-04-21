using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderMapTeleport : MonoBehaviour
{
    public Transform transformToTeleport; // The object to teleport

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.name} with tag: {other.tag}");
        if (other.CompareTag("Holster"))
        {
            Debug.Log($"Teleporting {other.name} to {transformToTeleport.position}");
            // Teleport the object to the specified transform's position and rotation
            other.transform.root.position = transformToTeleport.position;
            
            // Optionally, reset velocity if it's a Rigidbody
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }    
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.name} with tag: {other.tag}");
        if (other.CompareTag("Holster"))
        {
            Debug.Log($"Teleporting {other.name} to {transformToTeleport.position}");
            // Teleport the object to the specified transform's position and rotation
            other.transform.root.position = transformToTeleport.position;
            
            // Optionally, reset velocity if it's a Rigidbody
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }    
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"Trigger exited by: {other.name} with tag: {other.tag}");
        if (other.CompareTag("Holster"))
        {
            Debug.Log($"Teleporting {other.name} to {transformToTeleport.position}");
            // Teleport the object to the specified transform's position and rotation
            other.transform.root.position = transformToTeleport.position;
            
            // Optionally, reset velocity if it's a Rigidbody
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }    
    }
}
