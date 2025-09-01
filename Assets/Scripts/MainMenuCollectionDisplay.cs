using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class MainMenuCollectionDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject collectionItemPrefab;
    public List<CategoryUI> categoryUIs;

    [Header("Item Details UI")]
    [SerializeField] private GameObject itemDetailPanel;
    [SerializeField] private Image detailSprite;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private TextMeshProUGUI detailCategory;

    [Header("All Draggable Objects")]
    public List<DraggableObjectSOList> allDraggableObjectLists;

    [Header("Undiscovered Item Settings")]
    public Sprite unknownObjectSprite;
    public string unknownObjectName = "???";

    private List<DraggableObjectSO> collectedItems = new List<DraggableObjectSO>();
    private List<DraggableObjectSO> allDraggableObjects = new List<DraggableObjectSO>();
    private string saveFilePath;

    private void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerCollectionData.json");
        PopulateAllDraggableObjects();
        LoadCollectedItems();
        DisplayCollectedItems();
    }

    private void PopulateAllDraggableObjects()
    {
        foreach (var draggableObjectList in allDraggableObjectLists)
        {
            allDraggableObjects.AddRange(draggableObjectList.draggableObjectSOList);
            allDraggableObjects.AddRange(draggableObjectList.draggableObjectMergeSOList);
        }
    }

    private void LoadCollectedItems()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No save file found. Starting fresh.");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        CollectionSaveData saveData = JsonUtility.FromJson<CollectionSaveData>(json);

        foreach (string objName in saveData.discoveredObjectNames)
        {
            DraggableObjectSO foundObj = allDraggableObjects.Find(o => o.name == objName);
            if (foundObj != null)
            {
                collectedItems.Add(foundObj);
            }
        }

        Debug.Log("Collection loaded from: " + saveFilePath);
    }

    private void DisplayCollectedItems()
    {
        foreach (var categoryUI in categoryUIs)
        {
            foreach (Transform child in categoryUI.content)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (var draggableObjectSO in allDraggableObjects)
        {
            CategoryUI targetCategoryUI = categoryUIs.Find(c => c.objectCategory == GetObjectCategory(draggableObjectSO));

            if (targetCategoryUI != null)
            {
                GameObject item = Instantiate(collectionItemPrefab, targetCategoryUI.content);
                Image itemImage = item.GetComponentInChildren<Image>();
                TextMeshProUGUI itemName = item.GetComponentInChildren<TextMeshProUGUI>();

                if (collectedItems.Contains(draggableObjectSO))
                {
                    itemImage.sprite = draggableObjectSO.sprite;
                    itemImage.color = Color.white;
                    itemName.text = draggableObjectSO.name;

                    // Attach MainMenuCollectionItem script and pass item data
                    MainMenuCollectionItem itemUI = item.GetComponentInChildren<MainMenuCollectionItem>();
                    itemUI.Initialize(draggableObjectSO, this);
                }
                else
                {
                    itemImage.sprite = unknownObjectSprite;
                    itemName.text = unknownObjectName;
                }
            }
        }
    }

    public void DisplayItemDetails(DraggableObjectSO item)
    {
        detailSprite.sprite = item.sprite;
        detailName.text = item.name;
        detailDescription.text = item.description;
        detailCategory.text = $"Category: {item.GetObjectCategory()}";

        itemDetailPanel.SetActive(true);
    }

    public void CloseItemDetails()
    {
        itemDetailPanel.SetActive(false);
    }

    private ObjectCategory GetObjectCategory(DraggableObjectSO draggableObjectSO)
    {
        foreach (var draggableObjectList in allDraggableObjectLists)
        {
            if (draggableObjectList.draggableObjectSOList.Contains(draggableObjectSO) ||
                draggableObjectList.draggableObjectMergeSOList.Contains(draggableObjectSO))
            {
                return draggableObjectList.objectCategory;
            }
        }

        Debug.LogWarning($"Object {draggableObjectSO.name} not found in any category list!");
        return ObjectCategory.Home; // Use an unknown category instead of defaulting to something incorrect.
    }

}