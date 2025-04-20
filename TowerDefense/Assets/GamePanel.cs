using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanel : MonoBehaviour
{
    // Dont Destroy On Load
    private void Awake()
    {
        // Check if an instance already exists
        if (FindObjectsOfType<GamePanel>().Length > 1)
        {
            Destroy(gameObject); // Destroy this instance
            return;
        }
        
        DontDestroyOnLoad(gameObject); // Keep this instance alive across scenes
    }

    
}
