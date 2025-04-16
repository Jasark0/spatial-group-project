using UnityEngine;

public class BillboardText : MonoBehaviour
{
    private Transform cameraTransform;
    
    // Look for the main camera on start
    void Start()
    {
        // Check for VR camera first, then fallback to main camera
        var vrCamera = FindObjectOfType<OVRCameraRig>();
        if (vrCamera != null && vrCamera.centerEyeAnchor != null)
        {
            cameraTransform = vrCamera.centerEyeAnchor;
        }
        else
        {
            cameraTransform = Camera.main.transform;
        }
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            // Make text face the camera direction
            transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
                cameraTransform.rotation * Vector3.up);
        }
        else
        {
            // If camera was not found in Start, try again
            Start();
        }
    }
} 