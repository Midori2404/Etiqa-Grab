using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DraggableObject", menuName = "ScriptableObjects/DraggableObject")]
public class DraggableObjectSO : ScriptableObject
{
    public Sprite sprite;
    public int pointBonus;
    [TextArea(2,5)]
    public string description;
    private ObjectCategory objectCategory;

    public GameObject correctEffectPrefab;
    public float correctEffectScale = 10f;
    public GameObject wrongEffectPrefab;
    public float wrongEffectScale = 10f;

    public void SetObjectCategory(ObjectCategory objectCategory)
    {
        this.objectCategory = objectCategory;
    }

    public ObjectCategory GetObjectCategory()
    {
        return objectCategory;
    }
}
