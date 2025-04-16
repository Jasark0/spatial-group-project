using Oculus.Interaction.Locomotion;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    public GameObject missilePrefab;
    private ViewMode currentMode = ViewMode.FirstPerson;
    private float originalGravityFactor;
    [SerializeField] private CapsuleLocomotionHandler ovrPlayerController;
    private CapsuleCollider playerCollider;
    private Vector3 originalScale;
    private float planeSizeX;
    private float planeSizeZ;


    [Header(" Key Bindings ")]
    public OVRInput.Button keyBindForPovChange;
    public OVRInput.Button keyBindForMissileStrike;


    private void Start()
    {
        Debug.Log("Player started");
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
        // Toggle view when X button on left controller is pressed (secondary thumb stick button)
        if (OVRInput.GetDown(keyBindForPovChange))
        {
            Debug.Log("Toggle view");
            currentMode = currentMode == ViewMode.FirstPerson ? ViewMode.Build : ViewMode.FirstPerson;
            SetViewMode(currentMode);
        }

        if (gameManager.hasMissileStrike && OVRInput.GetDown(keyBindForMissileStrike))
        {
            // gameManager.hasMissileStrike = false;
            Debug.Log("Start missile strike");
            StartMissileStrike();
        }
    }

    public void SetViewMode(ViewMode mode)
    {
        if ( dartGun != null)
        {
            dartGun.gameObject.SetActive(mode == ViewMode.FirstPerson);
        }
        turretManager.gameObject.SetActive(mode == ViewMode.Build);

        if (mode == ViewMode.Build)
        {
            transform.localScale = new Vector3(
                buildHeight,
                buildHeight,
                buildHeight
            );
            ovrPlayerController.GravityFactor = 0;
            playerCollider.enabled = false;
        }
        else
        {
            transform.localScale = originalScale;
            ovrPlayerController.GravityFactor = originalGravityFactor;
            playerCollider.enabled = true;
        }
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