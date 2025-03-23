using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class UpgradePanelManager : MonoBehaviour
{
    public GameObject upgradePanel;
    public TMP_Text fireRateText;
    public TMP_Text healthText;
    public TMP_Text turretHealthText;
    public TMP_Text autoRepairText;
    private Turret selectedTurret;

    private GameManager gameManager;

    void Start()
    {
        upgradePanel.SetActive(false);
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
    if (upgradePanel.activeSelf && selectedTurret != null)
    {
        turretHealthText.text = $"Turret Health: {selectedTurret.health}";
    }
    if (Input.GetMouseButtonDown(1))
    {
        HideUpgradePanel();
    }
    }

    public void ShowUpgradePanel(Turret turret)
    {
        selectedTurret = turret;
        upgradePanel.SetActive(true);
        UpdatePanelInfo();
    }

    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
    }

    private void UpdatePanelInfo()
    {
        if (selectedTurret != null)
        {
            fireRateText.text = $"Level {selectedTurret.fireRateLevel} --> {selectedTurret.fireRateLevel + 1}";
            healthText.text = $"Level {selectedTurret.healthLevel} --> {selectedTurret.healthLevel + 1}";
            turretHealthText.text = $"Turret Health: {selectedTurret.health}";
            autoRepairText.text = $"Level {selectedTurret.autoRepairLevel} --> {selectedTurret.autoRepairLevel + 1}";
        }
    }

public void UpgradeFireRate()
{
    if (selectedTurret != null)
    {
        int upgradeCost = 100;
        if (gameManager.CanAfford(upgradeCost))
        {
            gameManager.DeductMoney(upgradeCost);
            selectedTurret.UpgradeFireRate();
            
            if (upgradePanel.activeSelf) 
                UpdatePanelInfo();
        }
    }
}

public void UpgradeHealth()
{
    if (selectedTurret != null)
    {
        int upgradeCost = 100;
        if (gameManager.CanAfford(upgradeCost))
        {
            gameManager.DeductMoney(upgradeCost);
            selectedTurret.UpgradeHealth();
            
            if (upgradePanel.activeSelf) 
                UpdatePanelInfo();
        }
    }
}

public void UpgradeAutoRepair()
{
 if (selectedTurret != null)
    {
        int upgradeCost = 100;
        if (gameManager.CanAfford(upgradeCost))
        {
            gameManager.DeductMoney(upgradeCost);
            selectedTurret.UpgradeAutoRepair(); 
            
            if (upgradePanel.activeSelf) 
                UpdatePanelInfo(); 
        }
    }
}
}
