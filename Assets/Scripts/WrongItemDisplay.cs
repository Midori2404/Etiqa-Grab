using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WrongItemDisplay : MonoBehaviour
{
    public TextMeshProUGUI incorrectItemName;
    public TextMeshProUGUI droppedOnThisCategory;
    public Image itemImage;
    public Image dropAreaImage; // New field for DropArea sprite

    public void Initialize(DraggableObjectSO item, ObjectCategory droppedCategory, Sprite dropAreaSprite)
    {
        if (itemImage != null) itemImage.sprite = item.sprite;
        incorrectItemName.text = $"{item.name} ({item.GetObjectCategory()})";
        droppedOnThisCategory.text = $"Dropped On: {droppedCategory}";
        if (dropAreaImage != null) dropAreaImage.sprite = dropAreaSprite; // Set DropArea sprite
    }
}

[System.Serializable]
public class IncorrectItemData
{
    public DraggableObjectSO item;
    public ObjectCategory droppedCategory;
    public Sprite dropAreaSprite; // New field for DropArea sprite

    public IncorrectItemData(DraggableObjectSO item, ObjectCategory droppedCategory, Sprite dropAreaSprite)
    {
        this.item = item;
        this.droppedCategory = droppedCategory;
        this.dropAreaSprite = dropAreaSprite;
    }
}