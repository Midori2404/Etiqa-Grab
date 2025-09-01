using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Placeholder : MonoBehaviour
{
    public DraggableObject trackedObject;
    public bool canMove = true;
    public float speed = 2f;

    public void Initialize(DraggableObject obj)
    {
        trackedObject = obj;
    }

    private void Update()
    {
        if (trackedObject != null && !trackedObject.IsBeingDragged())
        {
            trackedObject.transform.parent = this.transform;
            trackedObject.transform.localPosition = Vector3.zero;
        }
        else if (trackedObject != null && trackedObject.IsBeingDragged())
        {
            trackedObject.transform.parent = null;
        }

        if (!canMove)
            return;
        
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DestroyArea"))
        {
            if (trackedObject != null)
            {
                GameManager.Instance.ReduceHealth();
                Destroy(trackedObject.gameObject, 0.5f);
            }
            DestroyObject(0.5f);
        }
    }

    public void SetPlaceholderMovePermission(bool allow)
    {
        canMove = false;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void OnDestroy()
    {
        // Unsubscribe from event to prevent memory leaks
        ObjectSpawner.OnSpeedChanged -= SetSpeed;
    }

    public void DestroyObject(float duration)
    {
        // Destroy child object first if exist
        if (trackedObject != null)
        {
            Destroy(trackedObject.gameObject);
        }

        Destroy(gameObject, duration);
    }
}

