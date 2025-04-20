using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple script for handling the main menu navigation in VR.
/// </summary>
/// <remarks>
/// This component manages scene transitions between tutorial and main gameplay,
/// as well as providing application exit functionality.
/// /// </remarks>
public class OVRMainMenu : MonoBehaviour
{
    [SerializeField]
    private Button tutorialButton;
    
    [SerializeField]
    private Button mainGameButton;
    
    [SerializeField]
    private Button exitButton;

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject aboutPanel;

    // Scene indices
    private const int TUTORIAL_SCENE_INDEX = 2; // Change to your actual tutorial scene index
    private const int MAIN_GAME_SCENE_INDEX = 1; // Change to your actual main game scene index
    
    private const int MAIN_MENU_SCENE_INDEX = 0; // Change to your actual main menu scene index

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
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitApplication);
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

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE_INDEX);
        // Debug.Log("Main menu scene loaded");
    }

    /// <summary>
    /// Exits the application
    /// </summary>
    public void ExitApplication()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
        
        Debug.Log("Application exit requested");
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

    public void LoadAboutPanel()
    {
        if (mainMenuPanel != null && aboutPanel != null)
        {
            mainMenuPanel.SetActive(false);
            aboutPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Main menu or about panel is not assigned in the inspector.");
        }
    }

    public void LoadMainMenu()
    {
        if (mainMenuPanel != null && aboutPanel != null)
        {
            aboutPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Main menu or about panel is not assigned in the inspector.");
        }
    }
}