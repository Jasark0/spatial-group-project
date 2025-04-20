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
            Vector3 spawnPosition = player.transform.position + player.transform.forward * 0.5f + Vector3.up * 0.2f;
            Instantiate(gunPrefab, spawnPosition, player.transform.rotation);
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }
    }
}
