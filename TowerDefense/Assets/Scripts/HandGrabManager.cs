using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandGrabManager : MonoBehaviour
{

    public HandGrabInteractable[] interactables;
    
    private void Start()
    {
        foreach (var interactable in interactables)
        {
            if (interactable != null)
            {
                interactable.WhenPointerEventRaised += OnPointerEvent;
            }
        }
    }
    
    private void OnPointerEvent(PointerEvent evt)
    {
        // Only react to select (grab) and unselect (release) events
        if (evt.Type == PointerEventType.Select)
        {
            // Find which interactable was grabbed
            foreach (var interactable in interactables)
            {
                if (interactable != null && interactable.State == InteractableState.Select)
                {
                    // Disable all other interactables
                    foreach (var other in interactables)
                    {
                        if (other != null && other != interactable)
                        {
                            other.enabled = false;
                        }
                    }
                    break;
                }
            }
        }
        else if (evt.Type == PointerEventType.Unselect)
        {
            // When released, re-enable all interactables
            foreach (var interactable in interactables)
            {
                if (interactable != null)
                {
                    interactable.enabled = true;
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        foreach (var interactable in interactables)
        {
            if (interactable != null)
            {
                interactable.WhenPointerEventRaised -= OnPointerEvent;
            }
        }
    }
}
