using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml.Linq;

public class Merger : MonoBehaviour
{
    public static Merger Instance { get; private set; }
    private MergeManager mergeManager;
    private ObjectSpawner objectSpawner;
    private List<DraggableObject> objectsInMerger = new List<DraggableObject>();
    public bool isOccupied;
    AudioManager audioManager;

    [SerializeField] private DraggableObject currentMergedItem;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mergeManager = FindObjectOfType<MergeManager>();
        objectSpawner = ObjectSpawner.Instance;

        audioManager = AudioManager.instance;
    }

    public void AddToMerger(DraggableObject obj)
    {
        if (!objectsInMerger.Contains(obj) && currentMergedItem == null)
        {
            // If there is already one item in the merger, check if the new item can merge with it
            if (objectsInMerger.Count == 1)
            {
                DraggableObject existingObj = objectsInMerger[0];
                MergeRecipeSO recipe = mergeManager.TryMerge(existingObj.GetDraggableObjectSO(), obj.GetDraggableObjectSO());

                if (recipe == null)
                {
                    // No valid merge — snap back the new object
                    obj.SnapBackToPlaceholder();
                    Debug.Log("No valid merge. Snapping back to placeholder.");
                    audioManager.PlayInstance("MergeComplete");
                    return;
                }
            }

            // If valid or first item, add to merger
            objectsInMerger.Add(obj);
            CheckForMerge();
        }

        audioManager.PlayInstance("Merge");
    }


    public void RemoveFromMerger(DraggableObject obj)
    {
        if (objectsInMerger.Contains(obj))
        {
            objectsInMerger.Remove(obj);
            Debug.Log($"{obj.name} removed from Merger.");

            // Reset merger if no objects left
            if (objectsInMerger.Count == 0)
            {
                Debug.Log("Merger cleared. Ready for new merge.");
            }
        }

        if (currentMergedItem == obj)
        {
            currentMergedItem = null;
            // Stop Particle Effect
            GameObject mergedVFX = GameObject.Find("Stars_VFX");
            if (mergedVFX != null)
            {
                ParticleSystem mainVFX = mergedVFX.GetComponent<ParticleSystem>();
                if (mainVFX != null)
                {
                    mainVFX.Stop();
                }

                // Play all child particle systems
                foreach (ParticleSystem childPS in mergedVFX.GetComponentsInChildren<ParticleSystem>())
                {
                    childPS.Stop();
                }
            }
        }
    }


    private void CheckForMerge()
    {
        if (objectsInMerger.Count < 2) return;

        for (int i = 0; i < objectsInMerger.Count; i++)
        {
            for (int j = i + 1; j < objectsInMerger.Count; j++)
            {
                AttemptMerge(objectsInMerger[i], objectsInMerger[j]);
                return; // Only one merge at a time
            }
        }
    }

    public bool CanMergeWithExisting(DraggableObject newObj)
    {
        if (objectsInMerger.Count == 0) return true; // First item can always be added

        DraggableObject existingObj = objectsInMerger[0]; // Assume single existing item
        MergeRecipeSO recipe = mergeManager.TryMerge(existingObj.GetDraggableObjectSO(), newObj.GetDraggableObjectSO());

        return recipe != null;
    }


    public void AttemptMerge(DraggableObject objA, DraggableObject objB)
    {
        MergeRecipeSO recipe = mergeManager.TryMerge(objA.GetDraggableObjectSO(), objB.GetDraggableObjectSO());

        if (recipe != null)
        {
            PlayVFXBasedOnName("Merged_VFX");
            PlayVFXBasedOnName("Stars_VFX");
            audioManager.PlayInstance("MergeComplete");

            // Instantiate merged object
            GameObject mergedObj = Instantiate(objectSpawner.prefab, transform.position, Quaternion.identity);

            DraggableObject newDraggable = mergedObj.GetComponentsInChildren<DraggableObject>().FirstOrDefault(d => d.transform.parent == mergedObj.transform);
            newDraggable.Initialize(recipe.categoryList, recipe.result);

            // Setup Sprite
            SpriteRenderer spriteRenderer = mergedObj.GetComponentsInChildren<SpriteRenderer>().FirstOrDefault(s => s.transform.parent == mergedObj.transform);
            DraggableObjectSO draggableObjectSO = newDraggable.GetDraggableObjectSO();
            spriteRenderer.sprite = draggableObjectSO.sprite;

            // Lock movement
            newDraggable.SetMovingPermission(false);

            // Attach placeholder
            Placeholder placeholder = mergedObj.GetComponent<Placeholder>();
            placeholder.Initialize(newDraggable);
            placeholder.SetPlaceholderMovePermission(false);

            // Mark this area as occupied
            currentMergedItem = newDraggable;
            isOccupied = true;

            // Remove merged objects from list
            objectsInMerger.Remove(objA);
            objectsInMerger.Remove(objB);

            // Destroy original objects
            Destroy(objA.transform.parent.gameObject);
            Destroy(objB.transform.parent.gameObject);
            Debug.Log($"Merged into: {recipe.result.name}");
        }
    }

    public void ReassignMergedItem(DraggableObject obj)
    {
        if (currentMergedItem == null)
        {
            currentMergedItem = obj;
            obj.transform.position = transform.position;
            Debug.Log("Merged item reassigned to Merger.");
        }
    }

    private void PlayVFXBasedOnName(string vfxName)
    {
        // Play Particle Effect
        GameObject mergedVFX = GameObject.Find(vfxName);
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
    }
}