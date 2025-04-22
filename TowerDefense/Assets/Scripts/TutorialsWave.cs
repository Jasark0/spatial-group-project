using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class TutorialsWave : Waves
{
  private bool hasPlacedTurret = false;
  private bool shootPopupShown = false;
  private bool buildPopupShown = false;
  protected override void Start()
  {
    base.Start();
  }

  protected override void Update()
  {
    if (currentWave == 1)
    {
      if (!shootPopupShown)
      {
        PopUpManager.Instance.ShowPopUp(0, 10, "Buy a gun in the shop and shoot down the robots!");
        shootPopupShown = true;
      }
      base.Update();
    }
    else if (currentWave == 2)
    {
      if (!hasPlacedTurret)
      {
        // Check if player has placed a turret
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        if (turrets.Length > 0)
        {
          hasPlacedTurret = true;
          PopUpManager.Instance.ShowPopUp(0, 10, "Press A to launch a missile strike!");
        }
        else
        {
          if (!buildPopupShown)
          {
            PopUpManager.Instance.ShowPopUp(0, 10, "Buy a turret in the shop and place a turret!");
            buildPopupShown = true;
          }
        }
      }
      else
        base.Update();
    }
    else if (currentWave == 3)
    {
      // play one more wave before loading the main scene
      base.Update();
    }
    else
    {
      SceneManager.LoadScene("MainScene");
    }
  }


  // Override the GetRandomSpawnPosition method to only spawn from the right edge
  protected override Vector3 GetRandomSpawnPosition(GameObject balloon)
  {
    Vector3 planeCenter = planeTransform.position;
    float x = planeCenter.x + planeSizeX; // Right edge
    float z = Random.Range(planeCenter.z - planeSizeZ, planeCenter.z + planeSizeZ);
    float y = 0;

    // Keep the flying balloon height logic from the base class
    if (balloon == flyingBalloon)
    {
      y = Random.Range(15f, 30f);
    }

    return new Vector3(x, y, z);
  }
}