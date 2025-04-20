using UnityEngine;
using Oculus.Interaction;
using UnityEngine.UIElements;

public class WeaponHolsterRotationSystem : MonoBehaviour
{
    [SerializeField] private SnapInteractable _holsterSnapInteractable;
    [SerializeField] private Quaternion _pistolOffset = Quaternion.Euler(0, -90, 90);
    [SerializeField] private Quaternion _smgOffset = Quaternion.Euler(0, -90, 90);
    [SerializeField] private Vector3 _smgPositionOffset = new Vector3(0.0f, 0.0f, 0.0f); // Adjust as needed for SMG
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
                        weapon.GetComponent<Grabbable>().TransferOnSecondSelection = true; // Disable transfer on second selection for Rifle
                        AdjustSnappedRotation(transform, _rifleOffset);
                        currentlyHolsteredWeapon = weapon.gameObject; // Store the currently holstered weapon
                        _holsterCollider.enabled = false; // Disable the holster collider to prevent further interactions while weapon is snapped
                        // _holsterSnapInteractable.enabled = false; // Disable the holster snap interactable to prevent further interactions while weapon is snapped
                    }
                    else if (weapon.CompareTag("SMG"))
                    {
                        weapon.GetComponent<Grabbable>().TransferOnSecondSelection = true; // Disable transfer on second selection for SMG
                        AdjustSnappedRotation(transform, _smgOffset);
                        // AdjustSnappedPosition(transform, _smgPositionOffset); // Adjust position for SMG
                        
                        
                        currentlyHolsteredWeapon = weapon.gameObject; // Store the currently holstered weapon
                        _holsterCollider.enabled = false; // Disable the holster collider to prevent further interactions while weapon is snapped
                        // _holsterSnapInteractable.enabled = false; // Disable the holster snap interactable to prevent further interactions while weapon is snapped
                    }
                    else
                    {
                        Debug.LogWarning("Weapon type not recognized for rotation adjustment: " + weapon.tag);
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
                /* if ( weapon != null)
                {
                    Debug.LogWarning("Weapon un-snapped: " + weapon.tag);
                } */
                if (weapon.gameObject.tag == "SMG" || weapon.gameObject.tag == "Rifle")
                    weapon.TransferOnSecondSelection = false; // Re-enable transfer on second selection for the weapon

                // Re-enable logic here if needed
                transform.localRotation = Quaternion.identity; // Reset rotation when weapon is removed
                transform.localPosition = Vector3.zero; // Reset position when weapon is removed
                _holsterCollider.enabled = true; // enable the holster collider to prevent further interactions while weapon is snapped
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
        // Debug.Log("yo");
        holsterTransform.localRotation = offset;
    }

    private void AdjustSnappedPosition(Transform holsterTransform, Vector3 positionOffset)
    {
        holsterTransform.localPosition = positionOffset;
    }

}