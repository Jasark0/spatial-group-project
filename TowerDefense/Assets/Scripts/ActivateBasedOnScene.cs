using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivateBasedOnScene : MonoBehaviour
{
    [Header("Objects to Activate Based on Scene")]
    [SerializeField] private GameObject[] objectsToActivate; // Objects to activate in the current scene
    // Start is called before the first frame update

    [Header("Objects to Deactivate in based On Scenes")]
    [SerializeField] private GameObject[] objectsToDeactivate; // Objects to deactivate in the current scene
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            if (objectsToActivate[i] != null)
            {
                // Check if the object is in the current scene
                if (objectsToActivate[i].scene.name.Contains("Main Menu"))
                {
                    objectsToActivate[i].SetActive(true);
                }
                else
                {
                    objectsToActivate[i].SetActive(false);
                }
            }
        }

        for (int i = 0; i < objectsToDeactivate.Length; i++)
        {
            if (objectsToDeactivate[i] != null)
            {
                // Check if the object is in the current scene
                if (objectsToDeactivate[i].scene.name.Contains("Main Menu"))
                {
                    objectsToDeactivate[i].SetActive(false);
                }
                else
                {
                    objectsToDeactivate[i].SetActive(true);
                }
            }
        }
    }
}
