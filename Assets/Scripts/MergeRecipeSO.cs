using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MergeRecipe", menuName = "ScriptableObjects/MergeRecipe")]
public class MergeRecipeSO : ScriptableObject
{
    public DraggableObjectSO inputA;
    public DraggableObjectSO inputB;
    public DraggableObjectSO result;

    [Header("Category this item belongs to:")]
    public DraggableObjectSOList categoryList;
}
