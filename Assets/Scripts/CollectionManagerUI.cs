using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManagerUI : MonoBehaviour
{
    public CollectionUISpawner collectionUI;
    public List<CategoryUI> categoryUIs;
    public GameObject cheatSheetWindow;

    private void Start()
    {
        collectionUI = FindObjectOfType<CollectionUISpawner>();
        collectionUI.UpdateCollectionUI();
    }


    // Used in Unity OnClick event in Inspector
    public void OnCheatSheetButtonPressed()
    {
        cheatSheetWindow.SetActive(true);
        collectionUI.UpdateCollection(categoryUIs);
    }

    

    #region Item Description Display
    [SerializeField] private GameObject itemDetailPanel;
    [SerializeField] private Image detailSprite;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private TextMeshProUGUI detailCategory;

    public void ShowItemDetails(DraggableObjectSO itemData)
    {
        detailSprite.sprite = itemData.sprite;
        detailName.text = itemData.name;
        detailDescription.text = itemData.description;
        detailCategory.text = $"Category: {collectionUI.GetObjectCategory(itemData)}";

        itemDetailPanel.SetActive(true);
    }

    public void CloseItemDetails()
    {
        itemDetailPanel.SetActive(false);
    }


    #endregion
}

[System.Serializable]
public class CategoryUI
{
    public ObjectCategory objectCategory;
    public Transform content;
}