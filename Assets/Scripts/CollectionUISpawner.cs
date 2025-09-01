using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUISpawner : MonoBehaviour
{
    public Sprite unknownObjectSprite;
    public string unknownObjectName = "???";
    public GameObject collectionItemPrefab;
    public Transform collectionContent;

    public List<DraggableObjectSOList> draggableObjectSOLists;
    private CollectionManager collectionManager;
    private CollectionManagerUI collectionManagerUI;

    private void OnEnable()
    {
        CollectionManager.OnCollectionChanged += UpdateCollectionUI;
    }

    private void OnDisable()
    {
        CollectionManager.OnCollectionChanged -= UpdateCollectionUI;
    }

    private void Awake()
    {
        collectionManager = FindObjectOfType<CollectionManager>();
        collectionManagerUI = FindObjectOfType<CollectionManagerUI>();
        UpdateCollectionUI();
    }

    public void UpdateCollectionUI()
    {
        foreach (Transform child in collectionContent)
        {
            Destroy(child.gameObject);
        }

        List<DraggableObjectSO> allObjects = GetAllDraggableObjects();
        List<DraggableObjectSO> collectedObjects = collectionManager.GetCollection();

        if (collectedObjects == null || collectedObjects.Count == 0)
        {
            Debug.Log("no items discovered");
            collectedObjects = new List<DraggableObjectSO>();
        }

        foreach (var draggableObjectSO in allObjects)
        {
            GameObject item = Instantiate(collectionItemPrefab, collectionContent);
            Image itemImage = item.GetComponentInChildren<Image>();
            TextMeshProUGUI itemName = item.GetComponentInChildren<TextMeshProUGUI>();

            if (collectedObjects.Contains(draggableObjectSO))
            {
                itemImage.sprite = draggableObjectSO.sprite;
                itemImage.color = Color.white;
                itemName.text = draggableObjectSO.name;
            }
            else
            {
                itemImage.sprite = unknownObjectSprite;
                itemName.text = unknownObjectName;
            }
        }
    }

    public void UpdateCollection(List<CategoryUI> categoryUIs)
    {
        foreach (var categoryUI in categoryUIs)
        {
            foreach (Transform child in categoryUI.content)
            {
                Destroy(child.gameObject);
            }
        }

        List<DraggableObjectSO> allObjects = GetAllDraggableObjects();
        List<DraggableObjectSO> collectedObjects = collectionManager.GetCollection();

        foreach (var draggableObjectSO in allObjects)
        {
            CategoryUI targetCategoryUI = categoryUIs.Find(c => c.objectCategory == GetObjectCategory(draggableObjectSO));

            if (targetCategoryUI != null)
            {
                GameObject item = Instantiate(collectionItemPrefab, targetCategoryUI.content);
                Image itemImage = item.GetComponentInChildren<Image>();
                TextMeshProUGUI itemName = item.GetComponentInChildren<TextMeshProUGUI>();

                if (collectedObjects.Contains(draggableObjectSO))
                {
                    itemImage.sprite = draggableObjectSO.sprite;
                    itemImage.color = Color.white;
                    itemName.text = draggableObjectSO.name;

                    // Attach CollectionItem script and pass item data
                    CollectionItem collectionItem = item.GetComponentInChildren<CollectionItem>();
                    collectionItem.Initialize(draggableObjectSO, collectionManagerUI);
                }
                else
                {
                    itemImage.sprite = unknownObjectSprite;
                    itemName.text = unknownObjectName;
                }
            }
        }
    }


    private List<DraggableObjectSO> GetAllDraggableObjects()
    {
        List<DraggableObjectSO> allObjects = new List<DraggableObjectSO>();

        foreach (var draggableObjectSOList in draggableObjectSOLists)
        {
            allObjects.AddRange(draggableObjectSOList.draggableObjectSOList);        // Basic Items
            allObjects.AddRange(draggableObjectSOList.draggableObjectMergeSOList);   // Merged Items
        }

        return allObjects;
    }

    public ObjectCategory GetObjectCategory(DraggableObjectSO draggableObjectSO)
    {
        foreach (var draggableObjectSOList in draggableObjectSOLists)
        {
            if (draggableObjectSOList.draggableObjectSOList.Contains(draggableObjectSO) ||
                draggableObjectSOList.draggableObjectMergeSOList.Contains(draggableObjectSO))
            {
                return draggableObjectSOList.objectCategory;
            }
        }
        return ObjectCategory.Home; // Default category if not found
    }

}