using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DraggableObjectList", menuName = "ScriptableObjects/DraggableObjectList")]
public class DraggableObjectSOList : ScriptableObject
{
    public List<DraggableObjectSO> draggableObjectSOList;
    public List<DraggableObjectSO> draggableObjectMergeSOList;

    public ObjectCategory objectCategory;
}

public enum ObjectCategory
{
    Vehicle,
    Medical,
    Travel,
    Home
}