using UnityEngine;

public class GunShop : MonoBehaviour
{
    public GameObject smgPrefab;
    public GameObject arPrefab;

    public int smgCost = 500;
    public int arCost = 1000;

    public void BuySMG()
    {
        if (GameManager.Instance.CanAfford(smgCost))
        {
            GameManager.Instance.DeductMoney(smgCost);
            SpawnGun(smgPrefab);
        }
        else
        {
            Debug.Log("Not enough money for SMG");
        }
    }

    public void BuyAR()
    {
        if (GameManager.Instance.CanAfford(arCost))
        {
            GameManager.Instance.DeductMoney(arCost);
            SpawnGun(arPrefab);
        }
        else
        {
            Debug.Log("Not enough money for AR");
        }
    }

    private void SpawnGun(GameObject gunPrefab)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Find player's camera/view for proper positioning
            Transform cameraTransform = null;
            if (Player.Instance != null)
            {
                // Try to find the center eye anchor (VR camera)
                cameraTransform = Player.Instance.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");
                
                if (cameraTransform == null)
                {
                    // Fallback to camera rig if specific eye anchor isn't found
                    cameraTransform = Player.Instance.transform.Find("OVRCameraRig");
                }
            }

            if (cameraTransform != null)
            {
                // Get forward direction, keeping it horizontal
                Vector3 horizontalForward = cameraTransform.forward;
                horizontalForward.y = 0;
                
                // Handle edge case of looking straight up/down
                if (horizontalForward.magnitude < 0.1f)
                {
                    horizontalForward = Vector3.ProjectOnPlane(cameraTransform.up, Vector3.up).normalized;
                    if (horizontalForward.magnitude < 0.1f)
                    {
                        horizontalForward = Vector3.forward;
                    }
                }
                else
                {
                    horizontalForward = horizontalForward.normalized;
                }
                
                // Position gun at upper body height (slightly below eye level)
                float eyeHeight = cameraTransform.position.y;
                float upperBodyHeight = eyeHeight - 0.2f; // 20cm below eye level for upper body
                
                // Spawn position in front of player
                Vector3 spawnPosition = cameraTransform.position + horizontalForward * 0.7f; // 70cm in front
                spawnPosition.y = upperBodyHeight;
                
                // Spawn the gun with rotation facing the player for easy grabbing
                Quaternion spawnRotation = Quaternion.LookRotation(-horizontalForward, Vector3.up);
                spawnRotation *= Quaternion.Euler(-90f, 0f, 0f);
                // if (gunPrefab.tag == "SMG")
                // {
                //     // add 90 to rotation x
                //     spawnRotation *= Quaternion.Euler(90f, 0f, 0f);
                // }
                Instantiate(gunPrefab, spawnPosition, spawnRotation);
            }
            else
            {
                // Fallback if camera transform not found
                Vector3 spawnPosition = player.transform.position + player.transform.forward * 0.5f + Vector3.up * 1.4f;
                Instantiate(gunPrefab, spawnPosition, player.transform.rotation);
            }
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }
    }
}
