using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeAttack : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
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
