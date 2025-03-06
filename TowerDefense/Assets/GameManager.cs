using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public int money = 1000;
    public TMP_Text scoreText;
    public TMP_Text moneyText;

    public void UpdateScore(int points)
    {
        score += points;

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void UpdateMoney(int amount)
    {
        money += amount;

        if (moneyText != null)
        {
            moneyText.text = "Money: $" + money.ToString();
        }
    }
    public bool CanAfford(int cost)
    {
        return money >= cost;
    }

    public void DeductMoney(int cost)
    {
        if (CanAfford(cost))
        {
            money -= cost;
            UpdateMoney(0);
        }
    }
}

