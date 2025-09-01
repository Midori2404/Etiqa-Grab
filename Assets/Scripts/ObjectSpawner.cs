using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    #region Global Properties
    public static ObjectSpawner Instance { get; private set; }
    public static event Action<float> OnSpeedChanged; // Event to notify speed changes
    #endregion

    [Header("Spawner Properties")]
    public List<DraggableObjectSOList> draggableObjectSOList;
    public float spawnRate;

    [Header("Object Properties")]
    public GameObject prefab;
    public float objectSpeed;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);

            // Pick a random object from the list
            int indexForList = UnityEngine.Random.Range(0, draggableObjectSOList.Count);
            DraggableObjectSOList selectedSOList = draggableObjectSOList[indexForList];

            int indexForObject = UnityEngine.Random.Range(0, selectedSOList.draggableObjectSOList.Count);
            DraggableObjectSO selectedSOObject = selectedSOList.draggableObjectSOList[indexForObject];

            // Setting Up 
            var go = Instantiate(prefab, transform.position, Quaternion.identity);

            // Initialize the object with the corresponding data
            DraggableObject draggableObject = go.GetComponentInChildren<DraggableObject>();
            if (draggableObject != null)
            {
                // Setup Placeholder
                Placeholder placeholder = go.GetComponent<Placeholder>();
                placeholder.Initialize(draggableObject);

                // Setup Sprite and Name
                go.GetComponentInChildren<SpriteRenderer>().sprite = selectedSOObject.sprite;
                go.name = selectedSOObject.name;

                //Setup corresponding data and object speed
                draggableObject.Initialize(selectedSOList, selectedSOObject);
                placeholder.SetSpeed(objectSpeed);

                // Keep object speed in sync
                OnSpeedChanged += placeholder.SetSpeed;
            }
            else
            {
                Debug.LogError("Missing Component DraggableObject");
            }
        }
    }

    public void SetObjectSpeed(float newSpeed)
    {
        objectSpeed = newSpeed;
        OnSpeedChanged?.Invoke(objectSpeed);
        // Debug.Log($"Speed updated to: {objectSpeed}");
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnObjects());
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }
}
