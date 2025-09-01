using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour, IDraggableObject
{
    [SerializeField] private DraggableObjectSOList draggableObjectSOList;
    [SerializeField] private DraggableObjectSO draggableObjectSO;
    [SerializeField] private Placeholder placeholder;
    [SerializeField] private DropArea dropArea;
    [SerializeField] private Merger mergerArea;

    [SerializeField] private bool canDrop;
    [SerializeField] private bool onMergeArea;
    [SerializeField] private bool isDragged;
    [SerializeField] private float speed;

    AudioManager audioManager;

    public void Start()
    {
        audioManager = AudioManager.instance;
    }


    public void Initialize(DraggableObjectSOList draggableObjectSOList, DraggableObjectSO draggableObjectSO)
    {
        this.draggableObjectSOList = draggableObjectSOList;
        this.draggableObjectSO = draggableObjectSO;
        this.draggableObjectSO.SetObjectCategory(draggableObjectSOList.objectCategory);

        placeholder = GetComponentInParent<Placeholder>();
    }

    public void SetMovingPermission(bool allowed)
    {
        isDragged = allowed;
    }

    public int GetBonusPoint()
    {
        return draggableObjectSO.pointBonus;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DropArea"))
        {
            dropArea = other.GetComponent<DropArea>();
            canDrop = true;
        }
        else if (other.CompareTag("Merger"))
        {
            mergerArea = other.GetComponent<Merger>();
            onMergeArea = true; //  Set flag when inside Merger
            canDrop = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DropArea"))
        {
            dropArea = null;
            canDrop = false;
        }
        else if (other.CompareTag("Merger"))
        {
            onMergeArea = false; //  Clear flag when exiting Merger
            canDrop = false;
        }
    }


    public void SnapToDropArea()
    {
        if (canDrop)
        {
            if (dropArea != null)
            {
                transform.position = dropArea.transform.position;

                if (dropArea.GetAreaCategory() == draggableObjectSOList.objectCategory)
                {
                    //Play audio
                    audioManager.PlayInstance("CorrectSound");

                    Correct();
                }
                else
                {
                    //Play audio
                    audioManager.PlayInstance("WrongSound");

                    Incorrect();
                }

                if (mergerArea != null)
                {
                    mergerArea.RemoveFromMerger(this);
                }

                CollectionManager collectionManager = FindObjectOfType<CollectionManager>();
                collectionManager.DiscoverObject(draggableObjectSO);
                placeholder.DestroyObject(0);
            }
            else if (onMergeArea && mergerArea != null) // Use the onMergeArea flag
            {
                if (mergerArea.CanMergeWithExisting(this))
                {
                    placeholder.transform.position = mergerArea.transform.position;
                    transform.parent = placeholder.transform;
                    transform.localPosition = Vector3.zero;
                    isDragged = false;

                    placeholder.SetPlaceholderMovePermission(false);
                    mergerArea.AddToMerger(this);
                }
                else
                {
                    audioManager.PlayInstance("MergeWrong");
                    SnapBackToPlaceholder();
                    Debug.Log("Invalid merge, snapping back to conveyor.");
                }
            }
            else
            {
                SnapBackToPlaceholder();
            }
        }
        else
        {
            SnapBackToPlaceholder();
        }
    }




    public void SnapBackToPlaceholder()
    {
        transform.parent = placeholder.transform;
        transform.localPosition = Vector3.zero;
        isDragged = false;
    }


    public void Correct()
    {
        GameManager.Instance.AddPoint(GetBonusPoint());

        // Instantiate and play correct effect at object's position
        if (draggableObjectSO.correctEffectPrefab != null)
        {
            GameObject effect = Instantiate(draggableObjectSO.correctEffectPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
            effect.transform.localScale = Vector3.one * draggableObjectSO.correctEffectScale;
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }

            // Play child particle effects if they exist**
        foreach (ParticleSystem childPs in effect.GetComponentsInChildren<ParticleSystem>())
            {
                childPs.Play();
            }

            Destroy(effect, 2f); // Cleanup after playing
        }
    }

    public void Incorrect()
    {
        Debug.LogWarning("Incorrect Item Dropped");
        GameManager.Instance.StoreIncorrectItem(draggableObjectSO, dropArea.GetAreaCategory());

        // Instantiate and play wrong effect at object's position
        if (draggableObjectSO.wrongEffectPrefab != null)
        {
            GameObject effect = Instantiate(draggableObjectSO.wrongEffectPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
            effect.transform.localScale = Vector3.one * draggableObjectSO.wrongEffectScale;
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }

            // Play child particle effects if they exist**
        foreach (ParticleSystem childPs in effect.GetComponentsInChildren<ParticleSystem>())
            {
                childPs.Play();
            }

            Destroy(effect, 2f); // Cleanup after playing
        }
    }


    #region REFERENCES

    public DraggableObjectSO GetDraggableObjectSO()
    {
        return draggableObjectSO;
    }

    public ObjectCategory GetObjectCategory()
    {
        return draggableObjectSOList.objectCategory;
    }

    public bool IsDragged()
    {
        return isDragged;
    }

    private bool IsMergedItem()
    {
        return draggableObjectSOList.draggableObjectMergeSOList.Contains(draggableObjectSO);
    }


    #endregion
}

public static class DraggableObjectExtensions
{
    public static DraggableObjectSO GetDraggableObjectSO(this DraggableObject draggableObject)
    {
        return draggableObject.GetComponent<DraggableObject>().GetDraggableObjectSO();
    }

    public static bool IsBeingDragged(this DraggableObject draggableObject)
    {
        return draggableObject.GetComponent<DraggableObject>().IsDragged();
    }
}