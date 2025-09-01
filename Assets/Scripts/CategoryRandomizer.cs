using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryRandomizer : MonoBehaviour
{
    [System.Serializable]
    public class Table
    {
        public GameObject tableObject;
        public SpriteRenderer spriteRenderer;
        public DropArea dropArea; // Reference to DropArea component
    }

    public List<Table> tables = new List<Table>(); // List of tables
    public List<Sprite> logoSprites = new List<Sprite>(); // List of Logo sprites (Medical_Logo, Travel_Logo, House_Logo, Vehicle_Logo)
    public Dictionary<string, ObjectCategory> flagToCategory = new Dictionary<string, ObjectCategory>
    {
        { "Medical_Logo", ObjectCategory.Medical },
        { "Travel_Logo", ObjectCategory.Travel },
        { "House_Logo", ObjectCategory.Home },
        { "Vehicle_Logo", ObjectCategory.Vehicle }
    };

    void Start()
    {
        AssignRandomFlags();
    }

    void AssignRandomFlags()
    {
        if (logoSprites.Count < tables.Count)
        {
            Debug.LogError("Not enough sprites for all tables!");
            return;
        }

        List<Sprite> shuffledLogos = new List<Sprite>(logoSprites);
        ShuffleList(shuffledLogos);

        for (int i = 0; i < tables.Count; i++)
        {
            tables[i].spriteRenderer.sprite = shuffledLogos[i];
            string logoName = shuffledLogos[i].name;

            if (flagToCategory.TryGetValue(logoName, out ObjectCategory category))
            {
                tables[i].dropArea.SetAreaCategory(category);
            }
            else
            {
                Debug.LogWarning($"Flag name {logoName} not found in category mapping!");
            }
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    public Sprite GetCategorySprite(ObjectCategory category)
    {
        foreach (var sprite in logoSprites)
        {
            if (flagToCategory.TryGetValue(sprite.name, out ObjectCategory spriteCategory) && spriteCategory == category)
            {
                return sprite;
            }
        }
        return null;
    }
}