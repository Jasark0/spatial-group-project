using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeSpeed : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI buttonText; // Assign in Inspector
    private int currentLevel = 1;

    void Start()
    {
        UpgradeButtonText();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnButtonPress()
    {
        currentLevel++;
        UpgradeButtonText();
    }

    void UpgradeButtonText(){
        buttonText.text = $"level {currentLevel} -> {currentLevel + 1}";
    }
}
