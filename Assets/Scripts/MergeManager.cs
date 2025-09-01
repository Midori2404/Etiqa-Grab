using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    [SerializeField] private List<MergeRecipeSO> mergeRecipes;

    public MergeRecipeSO TryMerge(DraggableObjectSO objA, DraggableObjectSO objB)
    {
        foreach (var recipe in mergeRecipes)
        {
            if ((recipe.inputA == objA && recipe.inputB == objB) ||
                (recipe.inputA == objB && recipe.inputB == objA))
            {
                Debug.Log($"Merged {objA.name} and {objB.name} into {recipe.result.name}");
                return recipe;
            }
        }

        // Play Particle Effect
        GameObject mergedVFX = GameObject.Find("Wrong_Recipe_VFX");
        if (mergedVFX != null)
        {
            ParticleSystem mainVFX = mergedVFX.GetComponent<ParticleSystem>();
            if (mainVFX != null)
            {
                mainVFX.Play();
            }

            // Play all child particle systems
            foreach (ParticleSystem childPS in mergedVFX.GetComponentsInChildren<ParticleSystem>())
            {
                childPS.Play();
            }
        }

        Debug.Log("No valid merge found.");
        return null;
    }
}