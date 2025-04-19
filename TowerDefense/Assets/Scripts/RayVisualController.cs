using UnityEngine;

/// <summary>
/// Disables ray visual when controller grip button is pressed
/// </summary>
public class RayVisualController : MonoBehaviour
{
    [Tooltip("The ray visual GameObject to hide when gripping")]
    public GameObject rayVisual;
    
    [Tooltip("Which controller to check for grip input")]
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    
    [Tooltip("Should we use hand trigger or grip button?")]
    public bool useHandTrigger = true;

    private void Update()
    {
        if (rayVisual == null)
            return;

        // Check if grip/trigger is pressed
        bool isGripping = useHandTrigger ? 
            OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, controller) : 
            OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller);
        
        // Set ray visual active state based on grip state
        rayVisual.SetActive(!isGripping);
    }
}