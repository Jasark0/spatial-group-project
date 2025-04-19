using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager instance;

    public static PopUpManager Instance => instance;
    public GameObject[] popUps;

    private Transform cameraTransform;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        
        // Find the VR eye anchor transform at startup
        cameraTransform = FindCenterEyeAnchor();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPopUp(int index, float timer, string text = "no text inputed")
    {
        if (index < 0 || index >= popUps.Length)
        {
            Debug.LogError("PopUp index out of range: " + index);
            return;
        }

        Debug.Log("Showing PopUp: " + index + " with timer: " + timer + " and text: " + text);
        GameObject popUp = popUps[index];
        popUp.GetComponent<PopUpSettings>().timerDuration = timer;
        popUp.GetComponent<PopUpSettings>().popUpText = text;

        // Make sure we have a valid camera reference
        if (cameraTransform == null)
        {
            cameraTransform = FindCenterEyeAnchor();
            if (cameraTransform == null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        // Calculate rotation to face the player
        Quaternion lookRotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
        
        // Instantiate with the correct position and facing direction
        GameObject instance = Instantiate(popUp, transform.position, lookRotation, transform);
        instance.transform.localPosition += new Vector3(35, 180, 0);
        
        // Add a script that will continuously face the player
        instance.AddComponent<FaceCamera>();
    }
    
    private Transform FindCenterEyeAnchor()
    {
        // Try to find OVRCameraRig's center eye anchor
        var ovrCameraRig = FindObjectOfType<OVRCameraRig>();
        if (ovrCameraRig != null && ovrCameraRig.centerEyeAnchor != null)
        {
            return ovrCameraRig.centerEyeAnchor;
        }
        
        // Look for a GameObject named CenterEyeAnchor
        var centerEye = GameObject.Find("CenterEyeAnchor");
        if (centerEye != null)
        {
            return centerEye.transform;
        }
        
        // Fallback to main camera
        return Camera.main?.transform;
    }
}

// Add this class to the same file or create a new file
public class FaceCamera : MonoBehaviour
{
    private Transform cameraTransform;
    
    void Start()
    {
        // Find the camera
        var ovrCameraRig = FindObjectOfType<OVRCameraRig>();
        if (ovrCameraRig != null && ovrCameraRig.centerEyeAnchor != null)
        {
            cameraTransform = ovrCameraRig.centerEyeAnchor;
        }
        else
        {
            cameraTransform = Camera.main.transform;
        }
    }
    
    void Update()
    {
        if (cameraTransform != null)
        {
            // Make this object always face the camera
            Vector3 directionToCamera = transform.position - cameraTransform.position;
            if (directionToCamera != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(directionToCamera);
            }
        }
    }
}
