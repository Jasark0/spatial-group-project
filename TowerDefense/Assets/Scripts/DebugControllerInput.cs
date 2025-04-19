using UnityEngine;

public class DebugControllerInput : MonoBehaviour
{
    void Update()
    {
        // Check all possible buttons on both controllers
        CheckButtons(OVRInput.Controller.LTouch, "Left Controller");
        CheckButtons(OVRInput.Controller.RTouch, "Right Controller");

        // Check analog triggers & grips (since they can also act like buttons)
        CheckTriggerAndGrip(OVRInput.Controller.LTouch, "Left Controller");
        CheckTriggerAndGrip(OVRInput.Controller.RTouch, "Right Controller");

        // Check thumbstick touches/clicks
        CheckThumbstick(OVRInput.Controller.LTouch, "Left Controller");
        CheckThumbstick(OVRInput.Controller.RTouch, "Right Controller");
    }

    private void CheckButtons(OVRInput.Controller controller, string controllerName)
    {
        // Check all digital buttons
        if (OVRInput.GetDown(OVRInput.Button.One, controller))
            Debug.Log($"{controllerName}: Button.One (A/X) pressed");
        
        if (OVRInput.GetDown(OVRInput.Button.Two, controller))
            Debug.Log($"{controllerName}: Button.Two (B/Y) pressed");
        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, controller))
            Debug.Log($"{controllerName}: Thumbstick Click pressed");
        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
            Debug.Log($"{controllerName}: Grip pressed");
        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
            Debug.Log($"{controllerName}: Trigger pressed (digital)");
    }

    private void CheckTriggerAndGrip(OVRInput.Controller controller, string controllerName)
    {
        // Check analog trigger (if pulled past a threshold)
        float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);
        if (triggerValue > 0.5f)
            Debug.Log($"{controllerName}: Trigger pulled (analog: {triggerValue:F2})");

        // Check analog grip (if squeezed past a threshold)
        float gripValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);
        if (gripValue > 0.5f)
            Debug.Log($"{controllerName}: Grip squeezed (analog: {gripValue:F2})");
    }

    private void CheckThumbstick(OVRInput.Controller controller, string controllerName)
    {
        // Check thumbstick touch (not press)
        if (OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, controller))
            Debug.Log($"{controllerName}: Thumbstick touched");

        // Check thumbstick movement (2D axis)
        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller);
        if (thumbstick.magnitude > 0.1f)
            Debug.Log($"{controllerName}: Thumbstick moved (X: {thumbstick.x:F2}, Y: {thumbstick.y:F2})");
    }
}