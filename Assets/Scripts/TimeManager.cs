using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public float speed = 1.0f;
    public float speedIncreaseInterval = 30.0f;
    public Button CloseFunfact;

    private bool timerStarted = false;
    private float elapsedTime = 0f;

    private int bonusPoints = 0;

    public static TimeManager Instance { get; private set; }
    private UIManager uiManager;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        uiManager = UIManager.Instance;
        // Start countdown before enabling movement & timer
        CloseFunfact.onClick.AddListener(() => StartCoroutine(Countdown()));
    }

    private IEnumerator Countdown()
    {
        int countdown = 3;

        while (countdown > 0)
        {
            uiManager.UpdateCountdownText(countdown.ToString());
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        uiManager.UpdateCountdownText("GO!"); // Show "GO!" for 1 second

        yield return new WaitForSeconds(1f);

        timerStarted = true;

        uiManager.DisplayCountdownPanel(false); // Hide countdown panel

        ObjectSpawner.Instance.StartSpawning(); // Start spawning objects

        // Start speed increase loop
        StartIncreaseSpeed();
    }

    private void Update()
    {
        if (timerStarted)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeUI();

            if (elapsedTime >= 60f * (bonusPoints / 10 + 1))
            {
                bonusPoints += 10;
            }
        }
    }

    private void UpdateTimeUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        uiManager.UpdateTimer(minutes, seconds);
    }

    private void IncreaseSpeed()
    {
        ObjectSpawner.Instance.SetObjectSpeed(ObjectSpawner.Instance.objectSpeed + speed);
    }

    public void StopIncreaseSpeed()
    {
        CancelInvoke(nameof(IncreaseSpeed));
    }

    public void StartIncreaseSpeed()
    {
        InvokeRepeating(nameof(IncreaseSpeed), speedIncreaseInterval, speedIncreaseInterval);
    }

    public void StopTimer()
    {
        timerStarted = false;
    }
    public void StartTimer()
    {
        timerStarted = true;
    }

    public int GetBonusPoints()
    {
        return bonusPoints;
    }
}