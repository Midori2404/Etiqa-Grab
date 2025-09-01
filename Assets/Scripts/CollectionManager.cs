using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectionManager : MonoBehaviour
{
    public List<DraggableObjectSO> collectedObjects;

    public static event Action OnCollectionChanged;

    public void AddToCollection(DraggableObjectSO draggableObjectSO)
    {
        if (!collectedObjects.Contains(draggableObjectSO))
        {
            collectedObjects.Add(draggableObjectSO);
            Debug.Log($"Added {draggableObjectSO.name} to collection.");
            OnCollectionChanged?.Invoke();
        }
        else
        {
            Debug.Log($"{draggableObjectSO.name} is already in the collection.");
        }
    }

    public bool IsInCollection(DraggableObjectSO draggableObjectSO)
    {
        return collectedObjects.Contains(draggableObjectSO);
    }

    public void RemoveFromCollection(DraggableObjectSO draggableObjectSO)
    {
        if (collectedObjects.Contains(draggableObjectSO))
        {
            collectedObjects.Remove(draggableObjectSO);
            Debug.Log($"Removed {draggableObjectSO.name} from collection.");
            OnCollectionChanged?.Invoke();
        }
        else
        {
            Debug.Log($"{draggableObjectSO.name} is not in the collection.");
        }
    }

    public List<DraggableObjectSO> GetCollection()
    {
        if (collectedObjects == null)
            return new List<DraggableObjectSO>();
        else
            return collectedObjects;
    }

    public void DiscoverObject(DraggableObjectSO draggableObjectSO)
    {
        if (!collectedObjects.Contains(draggableObjectSO))
        {
            collectedObjects.Add(draggableObjectSO);
            Debug.Log($"Added {draggableObjectSO.name} to collection.");
            OnCollectionChanged?.Invoke();
        }
    }

}