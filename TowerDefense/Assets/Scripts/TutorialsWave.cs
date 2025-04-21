using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class TutorialsWave : Waves
{
  public TMP_Text tutorialPromptText;
  private bool waitingForTurretPlacement = false;
  private bool waitingForMissileStrike = false;
  private bool hasPlacedTurret = false;
  private GameManager gameManager;
  protected override void Start()
  {
    base.Start();

    gameManager = FindObjectOfType<GameManager>();
    if (tutorialPromptText != null)
    {
      tutorialPromptText.gameObject.SetActive(false);
    }
    //wait 5 seconds before loading the main scene
    Invoke("LoadMainScene", 5f);
  }

  void LoadMainScene()
  {
    SceneManager.LoadScene("MainScene");
  }

  void Update()
  {
    if (currentWave == 1)
    {
      if (tutorialPromptText != null && !tutorialPromptText.gameObject.activeSelf)
      {
        tutorialPromptText.text = "Shoot down the balloons with your dart gun!";
        tutorialPromptText.gameObject.SetActive(true);
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
          waitingForMissileStrike = true;
          if (tutorialPromptText != null)
          {
            tutorialPromptText.text = "Press A to launch a missile strike!";
          }
        }
        else if (tutorialPromptText != null)
        {
          tutorialPromptText.text = "Press B to enter build mode and place a turret!";
          tutorialPromptText.gameObject.SetActive(true);
        }
      }
      else
      {
        if (waitingForMissileStrike)
        {
          if (!gameManager.hasMissileStrike)
          {
            waitingForMissileStrike = false;
            tutorialPromptText.gameObject.SetActive(false);
          }
        }
        base.Update();
      }
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