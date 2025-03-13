using UnityEngine;

public class Player : MonoBehaviour
{
    private enum ViewMode
    {
        FirstPerson,
        Build
    }

    [SerializeField] private GameObject cameraRig;
    [SerializeField] private float buildHeight = 10f;
    [SerializeField] private DartGun dartGun;
    [SerializeField] private TurretPlacementManager turretManager;

    private ViewMode currentMode = ViewMode.FirstPerson;
    private float originalGravityModifier;
    private OVRPlayerController ovrPlayerController;
    private Vector3 originalScale;
    private void Start()
    {
        originalScale = transform.localScale;
        ovrPlayerController = GetComponent<OVRPlayerController>();
        Debug.Log("Player started");
        originalGravityModifier = ovrPlayerController.GravityModifier;
        SetViewMode(ViewMode.FirstPerson);
    }

    private void Update()
    {
        // Toggle view when B or Y button is pressed (secondary thumb stick button)
        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            Debug.Log("Toggle view");
            currentMode = currentMode == ViewMode.FirstPerson ? ViewMode.Build : ViewMode.FirstPerson;
            SetViewMode(currentMode);
        }
    }

    private void SetViewMode(ViewMode mode)
    {
        dartGun.gameObject.SetActive(mode == ViewMode.FirstPerson);
        turretManager.gameObject.SetActive(mode == ViewMode.Build);

        if (mode == ViewMode.Build)
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                buildHeight,
                transform.localScale.z
            );
            ovrPlayerController.GravityModifier = 0;
        }
        else
        {
            transform.localScale = originalScale;
            ovrPlayerController.GravityModifier = originalGravityModifier;
        }
    }
}