using UnityEngine;
using UnityEngine.XR;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel; 
    private bool isPaused = false;

    void Update()
    {
        if (IsXButtonPressed())
        {
            TogglePause();
        }
    }

    bool IsXButtonPressed()
    {
        UnityEngine.XR.InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        if (leftController.isValid && leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool isPressed))
        {
            return isPressed;
        }
        return false;
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ResumeGame()
    {
        isPaused = false; // Set the game as not paused
        pausePanel.SetActive(false); // Hide the pause panel
        Time.timeScale = 1; // Resume the game (set timeScale back to normal)
    }
}
