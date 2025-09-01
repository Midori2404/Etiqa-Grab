using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MergeRecipeSO))]
public class MergeRecipeSOEditor : Editor
{
    private Sprite selectedInputASprite;
    private Sprite selectedInputBSprite;
    private Sprite selectedResultSprite;

    public override void OnInspectorGUI()
    {
        MergeRecipeSO recipe = (MergeRecipeSO)target;

        EditorGUILayout.LabelField("Merge Recipe", EditorStyles.boldLabel);

        // Input A
        recipe.inputA = (DraggableObjectSO)EditorGUILayout.ObjectField("Input A", recipe.inputA, typeof(DraggableObjectSO), false);
        if (recipe.inputA != null && recipe.inputA.sprite != null)
        {
            DrawSingleSprite(recipe.inputA.sprite);
        }

        // Input B
        recipe.inputB = (DraggableObjectSO)EditorGUILayout.ObjectField("Input B", recipe.inputB, typeof(DraggableObjectSO), false);
        if (recipe.inputB != null && recipe.inputB.sprite != null)
        {
            DrawSingleSprite(recipe.inputB.sprite);
        }

        EditorGUILayout.Space();

        // Result
        recipe.result = (DraggableObjectSO)EditorGUILayout.ObjectField("Result", recipe.result, typeof(DraggableObjectSO), false);
        if (recipe.result != null && recipe.result.sprite != null)
        {
            DrawSingleSprite(recipe.result.sprite);
        }

        EditorGUILayout.Space();

        // Result SO List
        recipe.categoryList = (DraggableObjectSOList)EditorGUILayout.ObjectField("Category List", recipe.categoryList, typeof(DraggableObjectSOList), false);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(recipe);
        }
    }

    private void DrawSingleSprite(Sprite sprite)
    {
        if (sprite != null)
        {
            GUILayout.Label(AssetPreview.GetAssetPreview(sprite), GUILayout.Width(64), GUILayout.Height(64));
        }
    }
}
