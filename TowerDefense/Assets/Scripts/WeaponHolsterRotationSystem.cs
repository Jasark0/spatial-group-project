using UnityEngine;
using Oculus.Interaction;

public class WeaponHolsterRotationSystem : MonoBehaviour
{
    [SerializeField] private SnapInteractable _holsterSnapInteractable;
    [SerializeField] private Quaternion _pistolOffset = Quaternion.Euler(0, -90, 90);
    [SerializeField] private Quaternion _rifleOffset = Quaternion.identity;

    [SerializeField] private Transform transform;

    [SerializeField] private BoxCollider _holsterCollider;

    [SerializeField] private GameObject currentlyHolsteredWeapon;

    private void Start()
    {
        transform = this.GetComponent<Transform>();
        // Detect when a weapon is snapped into the holster
        _holsterSnapInteractable.WhenInteractorViewAdded += (interactorView) =>
        {
            // Get the interactor's GameObject, then check for Grabbable
            MonoBehaviour interactor = interactorView as MonoBehaviour;
            if (interactor != null)
            {
                Grabbable weapon = interactor.GetComponentInParent<Grabbable>();
                if (weapon != null)
                {
                    if (weapon.CompareTag("Pistol"))
                    {
                        AdjustSnappedRotation(transform, _pistolOffset);
                        currentlyHolsteredWeapon = weapon.gameObject; // Store the currently holstered weapon
                        _holsterCollider.enabled = false; // Disable the holster collider to prevent further interactions while weapon is snapped
                        // _holsterSnapInteractable.enabled = false; // Disable the holster snap interactable to prevent further interactions while weapon is snapped
                    }
                    else if (weapon.CompareTag("Rifle"))
                    {
                        AdjustSnappedRotation(transform, _rifleOffset);
                        currentlyHolsteredWeapon = weapon.gameObject; // Store the currently holstered weapon
                        _holsterCollider.enabled = false; // Disable the holster collider to prevent further interactions while weapon is snapped
                        // _holsterSnapInteractable.enabled = false; // Disable the holster snap interactable to prevent further interactions while weapon is snapped
                    }
                }

                
            }
        };

        // Re-enable weapon when un-snapped (optional)
        _holsterSnapInteractable.WhenInteractorViewRemoved += (interactorView) =>
        {
            MonoBehaviour interactor = interactorView as MonoBehaviour;
            if (interactor != null)
            {
                Grabbable weapon = interactor.GetComponentInParent<Grabbable>();
                // Re-enable logic here if needed
                transform.localRotation = Quaternion.identity; // Reset rotation when weapon is removed
                _holsterCollider.enabled = true; // Disable the holster collider to prevent further interactions while weapon is snapped
                currentlyHolsteredWeapon = null; // Clear the currently holstered weapon
                
                // _holsterSnapInteractable.enabled = true; // Re-enable the holster snap interactable for future interactions
            }
        };
    }

    void Update()
    {

    }

    private void AdjustSnappedRotation(Transform holsterTransform, Quaternion offset)
    {
        holsterTransform.localRotation = offset;
    }
}