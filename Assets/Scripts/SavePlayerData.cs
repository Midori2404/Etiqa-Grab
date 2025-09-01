using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SavePlayerData : MonoBehaviour
{
    public static SavePlayerData Instance { get; private set; }
    private GameManager gameManager;
    private string filePath;

    private void Awake()
    {
        Instance = this;

        filePath = Path.Combine(Application.persistentDataPath, "playerData.json");
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        LoadPlayerData();
    }

    private void LoadPlayerData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            gameManager.currentCoins = data.coins;
            UIManager.Instance.UpdateCoinText(gameManager.currentCoins);

            gameManager.highscore = data.highscore; // Load the stored highscore
            UIManager.Instance.UpdateCoinText(gameManager.currentCoins);
        }
    }

    public void SaveCoins()
    {
        if (gameManager.isLost)
        {
            // Determine the new highscore (if currentPoint is higher than the previous highscore)
            int newHighscore = gameManager.currentPoint;
            if (newHighscore < gameManager.highscore)
            {
                newHighscore = gameManager.highscore;
            }
            else
            {
                gameManager.highscore = newHighscore;
            }

            PlayerData data = new PlayerData
            {
                coins = gameManager.currentCoins + gameManager.CalculateCoins(),
                highscore = newHighscore
            };

            string json = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, json);

            MainMenu.Instance.BackToMenu();
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public int coins;
    public int highscore;
}
