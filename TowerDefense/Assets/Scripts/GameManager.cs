using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public int money = 1000;
    public TMP_Text[] scoreText;
    public TMP_Text[] moneyText;
    // public TMP_Text moneyTextVR;

    public int missileStrikeGoal = 1000;
    public bool hasMissileStrike = true;

    public void UpdateScore(int points)
    {
        score += points;

        // if (scoreText != null)
        // {
        //     scoreText.text = "Score: " + score;
        // }
        if ( scoreText != null)
        {
            foreach (TMP_Text text in scoreText)
            {
                text.text = "Score: " + score;
            }
        }

    

        if (score >= missileStrikeGoal)
        {
            hasMissileStrike = true;
            missileStrikeGoal *= 2;
        }
    }

    public void UpdateMoney(int amount)
    {
        money += amount;

        // if (moneyText != null)
        // {
        //     moneyText.text = "Money: $" + money.ToString();
        // }

        if (moneyText != null)
        {
            foreach (TMP_Text text in moneyText)
            {
                text.text = "Money: $" + money.ToString();
            }
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

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

