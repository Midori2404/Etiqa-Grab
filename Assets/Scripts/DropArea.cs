using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropArea : MonoBehaviour
{
    [SerializeField] private ObjectCategory areaCategory;
    
    public ObjectCategory GetAreaCategory()
    {
        return areaCategory;
    }

    public void SetAreaCategory(ObjectCategory category)
    {
        areaCategory = category;
    }
}
