using Oculus.Interaction.Locomotion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using Oculus.Interaction;

public class Player : MonoBehaviour
{
    public static Player instance;
    public static Player Instance => instance;

    // Define a delegate and event for view mode changes
    public delegate void ViewModeChangeHandler(ViewMode newMode);
    public event ViewModeChangeHandler OnViewModeChanged;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        // DontDestroyOnLoad(gameObject);
    }

    public enum ViewMode
    {
        FirstPerson,
        Build
    }

    [SerializeField] private GameObject cameraRig;
    [SerializeField] private float buildHeight = 10f;
    [SerializeField] private DartGun dartGun;
    [SerializeField] private TurretPlacementManager turretManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform planeTransform;

    [SerializeField] private GameObject HandGrabRight;
    [SerializeField] private GameObject HandGrabLeft;
    public GameObject missilePrefab;
    private ViewMode currentMode = ViewMode.FirstPerson;
    private float originalGravityFactor;
    [SerializeField] private CapsuleLocomotionHandler ovrPlayerController;
    private CapsuleCollider playerCollider;
    private Vector3 originalScale;
    private float planeSizeX;
    private float planeSizeZ;

    [Header("Grounding Check")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer; // Set this in Inspector to include your ground/floor layers

    [Header(" Key Bindings ")]
    public OVRInput.Button keyBindForPovChange;
    public OVRInput.Button keyBindForMissileStrike;


    private void Start()
    {
        // Debug.Log("Player started");
        originalScale = transform.localScale;
        playerCollider = ovrPlayerController.GetComponent<CapsuleCollider>();
        originalGravityFactor = ovrPlayerController.GravityFactor;
        SetViewMode(ViewMode.FirstPerson);

        if (planeTransform != null)
        {
            Renderer planeRenderer = planeTransform.GetComponent<Renderer>();
            if (planeRenderer != null)
            {
                planeSizeX = planeRenderer.bounds.size.x / 2f;
                planeSizeZ = planeRenderer.bounds.size.z / 2f;
            }
        }
    }

    private void Update()
    {
        // Debug.Log(IsGrounded() ? "Player is grounded" : "Player is not grounded");
        if ( SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "Main Menu 1")
        {
            // Disable player controls in the main menu scene
            return;
        }
        // Toggle view when X button on left controller is pressed (secondary thumb stick button)
        if (OVRInput.GetDown(keyBindForPovChange))
        {
            if (!IsHoldingAnything() && IsGrounded())
            {
                Debug.Log("Toggle view");
                currentMode = currentMode == ViewMode.FirstPerson ? ViewMode.Build : ViewMode.FirstPerson;
                SetViewMode(currentMode);
            }
            else if (!IsGrounded())
            {
                Debug.Log("Cannot switch view while not grounded");
            }
        }

        if (gameManager.hasMissileStrike && OVRInput.GetDown(keyBindForMissileStrike))
        {
            gameManager.hasMissileStrike = false;
            Debug.Log("Start missile strike");
            StartMissileStrike();
        }
    }

    public void SetViewMode(ViewMode mode)
    {
        // Check if player is holding anything before switching to build mode
        if (mode == ViewMode.Build && IsHoldingAnything())
        {
            Debug.Log("Cannot enter build mode while holding an object");
            return; // Don't change modes if holding something
        }

        // Check if player is grounded before switching to build mode
        if (mode == ViewMode.Build && !IsGrounded())
        {
            Debug.Log("Cannot enter build mode while not grounded");
            return; // Don't change modes if not grounded
        }

        // Continue with the existing view mode change logic
        if (dartGun != null)
        {
            dartGun.gameObject.SetActive(mode == ViewMode.FirstPerson);
        }

        if (turretManager != null)
            turretManager.gameObject.SetActive(mode == ViewMode.Build);

        if (HandGrabRight != null)
            HandGrabRight.SetActive(mode == ViewMode.FirstPerson);

        if (HandGrabLeft != null)
            HandGrabLeft.SetActive(mode == ViewMode.FirstPerson);
            
        // Update the current mode
        currentMode = mode;
            
        if (mode == ViewMode.Build)
        {
            transform.localScale = new Vector3(
                buildHeight,
                buildHeight,
                buildHeight
            );
            ovrPlayerController.GravityFactor = 0;
            playerCollider.enabled = false;
            // Get the camera
            Camera mainCamera = GameObject.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").GetComponent<Camera>();
            
            // Disable the TransparentFX layer in the culling mask
            int transparentFXLayer = LayerMask.NameToLayer("Tower");
            mainCamera.cullingMask &= ~(1 << transparentFXLayer);
            
            // Disable hand interactors in build mode
            DisableHandGrabInteractors();
        }
        else
        {
            transform.localScale = originalScale;
            ovrPlayerController.GravityFactor = originalGravityFactor;
            playerCollider.enabled = true;
            // Get the camera
            Camera mainCamera = GameObject.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").GetComponent<Camera>();
            
            // Enable the TransparentFX layer in the culling mask
            int transparentFXLayer = LayerMask.NameToLayer("Tower");
            mainCamera.cullingMask |= (1 << transparentFXLayer);
            // Re-enable hand interactors in first person mode
            EnableHandGrabInteractors();
        }
        
        // Notify subscribers about the mode change
        OnViewModeChanged?.Invoke(mode);
    }

    // Checks if the player is holding anything with either hand
    private bool IsHoldingAnything()
    {
        // Find all HandGrabInteractors in the player hierarchy
        Oculus.Interaction.HandGrab.HandGrabInteractor[] handGrabInteractors = 
            GetComponentsInChildren<Oculus.Interaction.HandGrab.HandGrabInteractor>();
        
        foreach (var interactor in handGrabInteractors)
        {
            // Check if this hand is actively grabbing something
            if (interactor.State == Oculus.Interaction.InteractorState.Select)
            {
                Debug.Log("Hand is holding an object, cannot switch to build mode");
                return true;
            }
        }
        
        return false;
    }

    // Disable all hand grab interactors
    private void DisableHandGrabInteractors()
    {
        // Find all HandGrabInteractors in the player hierarchy
        Oculus.Interaction.HandGrab.HandGrabInteractor[] handGrabInteractors = 
            GetComponentsInChildren<Oculus.Interaction.HandGrab.HandGrabInteractor>();
        
        foreach (var interactor in handGrabInteractors)
        {
            interactor.enabled = false;
        }
    }

    // Enable all hand grab interactors
    private void EnableHandGrabInteractors()
    {
        // Find all HandGrabInteractors in the player hierarchy
        Oculus.Interaction.HandGrab.HandGrabInteractor[] handGrabInteractors = 
            GetComponentsInChildren<Oculus.Interaction.HandGrab.HandGrabInteractor>();
        
        foreach (var interactor in handGrabInteractors)
        {
            interactor.enabled = true;
        }
    }

    // Check if the player is standing on the ground
    private bool IsGrounded()
    {
        if (playerCollider == null) return false;
        
        // Use OverlapCapsule to check for collision with ground layer
        // Get capsule dimensions and position
        Vector3 point0 = transform.position + playerCollider.center + Vector3.up * (playerCollider.height / 2 - playerCollider.radius);
        Vector3 point1 = transform.position + playerCollider.center + Vector3.down * (playerCollider.height / 2 - playerCollider.radius);
        float radius = playerCollider.radius * 0.95f; // Use slightly smaller radius to avoid detecting walls
        
        // Check if the bottom of the capsule is overlapping with ground objects
        Collider[] colliders = Physics.OverlapCapsule(
            point0, 
            point1, 
            radius,
            groundLayer
        );
        
        // If we found any ground colliders, we're grounded
        return colliders.Length > 0;
    }

    public bool IsInBuildMode()
    {
        return currentMode == ViewMode.Build;
    }

    public void StartMissileStrike()
    {
        Vector3 planeCenter = planeTransform.position;
        for (int i = 0; i < 20; i++)
        {
            float x = Random.Range(planeCenter.x - planeSizeX, planeCenter.x + planeSizeX);
            float z = Random.Range(planeCenter.z - planeSizeZ, planeCenter.z + planeSizeZ);
            Vector3 randomPosition = new(x, 100, z);
            GameObject missile = Instantiate(missilePrefab, randomPosition, Quaternion.identity);
            missile.transform.localScale = new Vector3(2f, 2f, 2f);
            missile.GetComponent<Bullet>().Init(Vector3.down, 300, "Turret");
        }
    }
}