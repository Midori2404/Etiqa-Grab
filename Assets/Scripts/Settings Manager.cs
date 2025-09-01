using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SettingsManager : MonoBehaviour
{
    public Image buttonImage; 
    public Sprite soundOnSprite; 
    public Sprite soundOffSprite;

    void Start()
    {
        UpdateButtonSprite(); 
    }

    public void ToggleSound()
    {
        AudioManager.instance.ToggleMute();
        UpdateButtonSprite();
    }

    void UpdateButtonSprite()
    {
        bool isMuted = AudioManager.instance.IsMuted();
        buttonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
    }
}


