using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class GrabInteractorBypass : GrabInteractor
{

    protected override void DoPostprocess()
    {
        base.DoPostprocess();
        
        // Allow thumbstick input to pass through to other systems even during grab
        if (State == InteractorState.Select)
        {
            Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
            if (Mathf.Abs(thumbstick.x) > 0.5f)
            {
                // Let thumbstick events propagate to other systems
                // This is a hack that ensures the input isn't consumed fully by this interactor
                OVRInput.GetUp(OVRInput.Button.Any); 

          
            }
            

        }
        else
        {
            // Enable ray when not grabbing
           /*  if (rayObject != null)
            {
                rayObject.SetActive(true);
            } */
        }
    }
}
