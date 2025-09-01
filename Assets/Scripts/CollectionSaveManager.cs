using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class CollectionSaveData
{
    public List<string> discoveredObjectNames = new List<string>();
}

public class CollectionSaveManager : MonoBehaviour
{
    private string saveFilePath;
    private CollectionManager collectionManager;

    private void Awake()
    {
        // Define the file path inside the new directory
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerCollectionData.json");
        collectionManager = FindObjectOfType<CollectionManager>();
    }

    private void Start()
    {
        LoadCollection();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SaveCollection();
        }
    }

    public void SaveCollection()
    {
        CollectionSaveData saveData = new CollectionSaveData();

        foreach (var obj in collectionManager.GetCollection())
        {
            saveData.discoveredObjectNames.Add(obj.name);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Collection saved to: " + saveFilePath);
    }

    public void LoadCollection()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No save file found. Starting fresh.");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        CollectionSaveData saveData = JsonUtility.FromJson<CollectionSaveData>(json);

        List<DraggableObjectSO> allObjects = GetAllDraggableObjects();

        foreach (string objName in saveData.discoveredObjectNames)
        {
            DraggableObjectSO foundObj = allObjects.Find(o => o.name == objName);
            if (foundObj != null)
            {
                collectionManager.AddToCollection(foundObj);
            }
        }

        Debug.Log("Collection loaded from: " + saveFilePath);
    }

    private List<DraggableObjectSO> GetAllDraggableObjects()
    {
        List<DraggableObjectSO> allObjects = new List<DraggableObjectSO>();
        CollectionUISpawner collectionUI = FindObjectOfType<CollectionUISpawner>();

        foreach (var list in collectionUI.draggableObjectSOLists)
        {
            allObjects.AddRange(list.draggableObjectSOList);        // Basic Items
            allObjects.AddRange(list.draggableObjectMergeSOList);   // Merged Items
        }

        return allObjects;
    }


    private void OnApplicationQuit()
    {
        SaveCollection();
    }
}
