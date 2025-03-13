using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TurretPlacementManager turretPlacementManager;
    public Button[] turretButtons;

    void Start()
    {
        for (int i = 0; i < turretButtons.Length; i++)
        {
            int index = i;
            turretButtons[i].onClick.AddListener(() => OnTurretButtonClick(index));
        }
    }

    public void OnTurretButtonClick(int index)
    {
        turretPlacementManager.SelectTurret(index);
    }
}
