using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionItem : MonoBehaviour
{
    private DraggableObjectSO itemData;
    private CollectionManagerUI collectionUISpawner;

    [SerializeField] private Button button;

    public void Awake()
    {
        button.onClick.AddListener(ShowItemDetails);
    }

    public void Initialize(DraggableObjectSO data, CollectionManagerUI ui)
    {
        itemData = data;
        collectionUISpawner = ui;
    }

    public void ShowItemDetails()
    {
        collectionUISpawner.ShowItemDetails(itemData);
    }
}
