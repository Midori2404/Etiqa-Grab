using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCollectionItem : MonoBehaviour
{
    public Image itemImage;
    private DraggableObjectSO item;
    private MainMenuCollectionDisplay displayManager;
    
    public Button button;

    public void Awake()
    {
        button.onClick.AddListener(OnItemClicked);
    }

    public void Initialize(DraggableObjectSO item, MainMenuCollectionDisplay displayManager)
    {
        this.item = item;
        this.displayManager = displayManager;
        itemImage.sprite = item.sprite;
    }

    private void OnItemClicked()
    {
        displayManager.DisplayItemDetails(item);
    }
}