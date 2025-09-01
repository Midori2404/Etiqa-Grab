using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private UIManager uiManager;
    private TimeManager timerManager;
    private CategoryRandomizer categoryRandomizer;
    AudioManager audioManager;

    [Header("Game Properties")]
    public int currentPoint;
    public int currentCoins;
    public int currentHealth = 3;
    public List<GameObject> heartContainers;
    public bool isLost;
    public int highscore;

    [Header("Refs")]
    public GameObject ButtonPay;
    public GameObject TextPay;

    private int continueCount = 0;
    private const int maxContinues = 2;
    private const int continueCost = 5;
    private const int continueHealth = 3;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    [Header("Display Wrong Items Settings")]
    private List<IncorrectItemData> incorrectItems = new List<IncorrectItemData>();
    public GameObject wrongItemPrefab;
    public Transform wrongItemContainer;

    private float previousObjectSpeed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        uiManager = UIManager.Instance;
        timerManager = TimeManager.Instance;
        audioManager = AudioManager.instance;
        categoryRandomizer = FindObjectOfType<CategoryRandomizer>();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    #region Points Handler
    public void AddPoint(int point)
    {
        int bonusPoints = timerManager.GetBonusPoints();
        int calculatedPoints = bonusPoints + point;
        currentPoint += calculatedPoints;
        uiManager.UpdatePointText(currentPoint);
    }

    public void DecreasePoint(int point)
    {
        currentPoint -= point;
        uiManager.UpdatePointText(currentPoint);
    }

    public void AddCorrectDropPoints()
    {
        int basePoints = 10;
        int bonusPoints = timerManager.GetBonusPoints();
        AddPoint(basePoints + bonusPoints);
    }
    #endregion

    #region Health Handler
    public void AddHealth()
    {
        if (currentHealth < 3)
        {
            currentHealth++;
            heartContainers[currentHealth - 1].SetActive(true);
        }
    }

    public void ReduceHealth()
    {
        if (currentHealth > 0)
        {
            heartContainers[currentHealth - 1].SetActive(false);
            currentHealth--;

            //Audioplay
            audioManager.PlayInstance("FiredSound");

            if (currentHealth == 0 && !isLost)
            {
                GameOver();
            }
        }
    }
    #endregion

    #region Points to Coins Conversion
    public int CalculateCoins()
    {
        int coins = currentPoint / 100;
        return coins;
    }
    #endregion

    #region Handle Wrong Item Display
    public void StoreIncorrectItem(DraggableObjectSO item, ObjectCategory droppedCategory)
    {
        Sprite dropAreaSprite = categoryRandomizer.GetCategorySprite(droppedCategory);
        incorrectItems.Add(new IncorrectItemData(item, droppedCategory, dropAreaSprite));
    }

    public void GameOver()
    {
        //Audioplay
        audioManager.PlayInstance("GameOverSound");

        // Stop Item Spawn
        ObjectSpawner.Instance.StopSpawning();

        // Stop Speed Increase based from timer
        timerManager.StopTimer();
        timerManager.StopIncreaseSpeed();

        // Identifier Boolean
        isLost = true;

        // Set all object to stop moving
        previousObjectSpeed = ObjectSpawner.Instance.objectSpeed;
        ObjectSpawner.Instance.SetObjectSpeed(0);

        CollectionSaveManager collectionSaveManager = FindObjectOfType<CollectionSaveManager>();
        collectionSaveManager.SaveCollection();

        if (continueCount == maxContinues)
        {
            ButtonPay.SetActive(false);
            TextPay.SetActive(false);
        }
        // Show Game Over screen
        gameOverPanel.SetActive(true);
    }

    // In Unity Event Button
    public void SetupPointAndCoins()
    {
        // Start Point to Coin Animation
        int coinsEarned = currentPoint / 100;
        StartCoroutine(GameOverUI.Instance.AnimatePointToCoin(currentPoint, coinsEarned));

        DisplayIncorrectItems();

        // Update highscore if the current score is greater than the stored highscore
        if (currentPoint > highscore)
        {
            highscore = currentPoint;
        }
        // Update the UI with the current highscore.
        UIManager.Instance.UpdateHighscoreText(highscore);
    }

    public void DisplayIncorrectItems()
    {
        // Clear previous incorrect item UI elements
        foreach (Transform child in wrongItemContainer)
        {
            Destroy(child.gameObject);
        }

        // Instantiate WrongItemDisplay prefabs for each incorrect item
        foreach (IncorrectItemData data in incorrectItems)
        {
            GameObject wrongItemObj = Instantiate(wrongItemPrefab, wrongItemContainer);
            WrongItemDisplay wrongItemDisplay = wrongItemObj.GetComponent<WrongItemDisplay>();
            wrongItemDisplay.Initialize(data.item, data.droppedCategory, data.dropAreaSprite);
        }

        // Reset Incorrect Items
        // incorrectItems.Clear();
    }
    #endregion

    #region Handle Game Over Settings

    public void ContinueGame()
    {
        if (continueCount < maxContinues && currentCoins >= continueCost)
        {
            currentCoins -= continueCost; // Deduct coins for continuing the game
            uiManager.UpdateCoinText(currentCoins);

            isLost = false;
            gameOverPanel.SetActive(false);

            // Reset Health
            currentHealth = continueHealth;
            for (int i = 0; i < currentHealth; i++)
            {
                heartContainers[i].SetActive(true);
            }

            continueCount++;
            StartCoroutine(ContinueGameCountdown());
        }
        else
        {
            ButtonPay.SetActive(false);
            TextPay.SetActive(false);
            Debug.Log("Not enough coins to continue or maximum continues reached.");
        }
    }

    private IEnumerator ContinueGameCountdown()
    {
        UIManager.Instance.DisplayCountdownPanel(true);
        // Display countdown (e.g., 3, 2, 1)
        for (int i = 3; i > 0; i--)
        {
            uiManager.UpdateCountdownText(i.ToString());
            yield return new WaitForSeconds(1f);
        }

        uiManager.UpdateCountdownText("GO!");

        yield return new WaitForSeconds(1f);

        UIManager.Instance.DisplayCountdownPanel(false);

        // Restore object speed
        ObjectSpawner.Instance.SetObjectSpeed(previousObjectSpeed);

        // Restore speed increase loop
        timerManager.StartTimer();
        timerManager.StartIncreaseSpeed();

        // Start spawning objects again
        ObjectSpawner.Instance.StartSpawning();
    }

    public void OnApplicationQuit()
    {
        SavePlayerData.Instance.SaveCoins();
    }

    public void OnReturnToMenu()
    {
        SavePlayerData.Instance.SaveCoins();
    }
    #endregion

    #region Handle Pause Settings
    public void PauseGame()
    {
        // Stop Item Spawn
        ObjectSpawner.Instance.StopSpawning();

        // Stop Speed Increase based from timer
        timerManager.StopTimer();
        timerManager.StopIncreaseSpeed();

        // Set all object to stop moving
        previousObjectSpeed = ObjectSpawner.Instance.objectSpeed;
        ObjectSpawner.Instance.SetObjectSpeed(0);

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        // Restore object speed
        ObjectSpawner.Instance.SetObjectSpeed(previousObjectSpeed);

        // Restore speed increase loop
        timerManager.StartTimer();
        timerManager.StartIncreaseSpeed();

        // Start spawning objects again
        ObjectSpawner.Instance.StartSpawning();
    }
    #endregion
}