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
    public GameObject missilePrefab;
    private ViewMode currentMode = ViewMode.FirstPerson;
    private float originalGravityFactor;
    [SerializeField] private CapsuleLocomotionHandler ovrPlayerController;
    private Vector3 originalScale;
    private void Start()
    {
        originalScale = transform.localScale;
        Debug.Log("Player started");
        originalGravityFactor = ovrPlayerController.GravityFactor;
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

        if (gameManager.hasMissileStrike && OVRInput.GetDown(OVRInput.RawButton.A))
        {
            // gameManager.hasMissileStrike = false;
            Debug.Log("Start missile strike");
            StartMissileStrike();
        }
    }

    public void SetViewMode(ViewMode mode)
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
            ovrPlayerController.GravityFactor = 0;
        }
        else
        {
            transform.localScale = originalScale;
            ovrPlayerController.GravityFactor = originalGravityFactor;
        }
    }

    public void StartMissileStrike()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 randomPosition = new(Random.Range(-50, 50), 100, Random.Range(-50, 50) - 14);
            GameObject missile = Instantiate(missilePrefab, randomPosition, Quaternion.identity);
            missile.transform.localScale = new Vector3(2f, 2f, 2f);
            missile.GetComponent<Bullet>().Init(Vector3.down, 300, "Turret");
        }
    }
}