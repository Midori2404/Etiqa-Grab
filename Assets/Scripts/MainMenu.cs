using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public string Scene;
    public Button NoSpam;
    public Animator transition;

    AudioManager audioManager;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    { 
        audioManager = AudioManager.instance;
    }

    void Update()
    {

    }

    public void ChangeScene()
    {
        if (!NoSpam.interactable) return;

        NoSpam.interactable = false;
        Invoke(nameof(ReenableButton), 1f);

        Debug.Log("Pressed");
        StartCoroutine(OpenScene());
        //SceneManager.LoadScene("Temporary");
        PlayerPrefs.SetInt("AttackDamage", 0);
        PlayerPrefs.SetInt("Defense", 0);
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("ScoreCoins", 0);
        PlayerPrefs.SetInt("ScoreGems", 0);
        PlayerPrefs.SetInt("ScoreStars", 0);
        //audioManager.PlayInstance("ButtonClick");
    }

    private void ReenableButton()
    {
        NoSpam.interactable = true; // Enable button again
    }


    IEnumerator OpenScene()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(Scene);
    }


    public void ExitPlay()
    {
        audioManager.PlayInstance("ButtonClick");

        Debug.Log("Exit");
        Application.Quit();
    }

    public void SelectSound()
    {
        audioManager.PlayInstance("ButtonClick");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
}
