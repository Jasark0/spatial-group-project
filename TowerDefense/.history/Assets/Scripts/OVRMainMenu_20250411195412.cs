using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OVRMainMenu : MonoBehaviour
{
    [SerializeField]
    private Button tutorialButton;
    
    [SerializeField]
    private Button mainGameButton;

    // Scene indices
    private const int TUTORIAL_SCENE_INDEX = 0; // Change to your actual tutorial scene index
    private const int MAIN_GAME_SCENE_INDEX = 1; // Change to your actual main game scene index

    void Start()
    {
        // Set up button listeners
        if (tutorialButton != null)
        {
            tutorialButton.onClick.AddListener(LoadTutorialScene);
        }
        
        if (mainGameButton != null)
        {
            mainGameButton.onClick.AddListener(LoadMainGameScene);
        }
    }

    /// <summary>
    /// Loads the tutorial scene
    /// </summary>
    public void LoadTutorialScene()
    {
        SceneManager.LoadScene(TUTORIAL_SCENE_INDEX);
    }

    /// <summary>
    /// Loads the main game scene
    /// </summary>
    public void LoadMainGameScene()
    {
        SceneManager.LoadScene(MAIN_GAME_SCENE_INDEX);
    }

    // For direct scene loading by index (can be called from button events in Unity Editor)
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // For loading scene by name (alternative approach)
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}