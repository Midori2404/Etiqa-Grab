using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Timer")]
    public TextMeshProUGUI timeText;
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;

    [Header("Points")]
    public TextMeshProUGUI pointText;
    public TextMeshProUGUI highScoreAmount;

    [Header("Coins")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI coinsTextInGameOver;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void UpdatePointText(int point)
    {
        pointText.text = "Points: " + point.ToString();
    }

    public void UpdateCoinText(int coin)
    {
        coinText.text = "Coins: " + coin.ToString();
        coinsTextInGameOver.text = "Your Coins: " + coin.ToString();
    }

    public void UpdateTimer(float minute, float second)
    {
        timeText.text = minute.ToString("F0") + ":" + second.ToString("F0");
    }

    public void UpdateCountdownText(string time)
    {
        countdownText.text = time;
    }

    public void DisplayCountdownPanel(bool display)
    {
        countdownPanel.SetActive(display);
    }

    public void UpdateHighscoreText(int highscore)
    {
        highScoreAmount.text = highscore.ToString();
    }
}
