using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpSettings : MonoBehaviour
{
    public float timerDuration = 5f; // Duration for the pop-up to be visible
    public string popUpText;

    private TextMeshProUGUI textMeshProUGUI;
    // Start is called before the first frame update
    void Start()
    {
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.text = popUpText;
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component not found in children of PopUpSettings.");
        }
        timerDuration += 1; // Add 1 second to the timer to account for the pop-up animation
        StartCoroutine(PopUpCountdown());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PopUpCountdown()
    {
        yield return new WaitForSeconds(timerDuration);

        this.GetComponent<Animator>().SetTrigger("PopOff");
        yield return new WaitForSeconds(1.1f); // Wait for the pop-off animation to finish
        Destroy(gameObject); // Destroy the pop-up after the timer expires
    }

    
}
