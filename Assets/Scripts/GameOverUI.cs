using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;
    [Header("Coins & Points UI")]
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI coinsText;

    [Header("Continue UI")]
    public Button payButton;

    private int coinsEarned;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public IEnumerator AnimatePointToCoin(int startingPoints, int totalCoinsEarned)
    {
        int currentPoints = startingPoints;
        int coinsGained = 0;

        while (currentPoints >= 100)
        {
            currentPoints -= 100;
            coinsGained += 1;

            pointsText.text = currentPoints.ToString();
            coinsText.text = coinsGained.ToString();

            yield return new WaitForSeconds(0.1f); // Smooth animation speed
        }

        // Final update
        pointsText.text = currentPoints.ToString();
        coinsText.text = coinsGained.ToString();

        coinsEarned = coinsGained; // Store the earned coins for display
    }

    public void OnReturnToMenuButtonPressed()
    {
        SavePlayerData.Instance.SaveCoins();
    }

    public void OnPayButtonPressed()
    {
        GameManager.Instance.ContinueGame();
    }
}