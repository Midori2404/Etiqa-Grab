using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelDrag : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 panelLocation;
    public float percentThreshold = .2f;
    public float Ease = .5f;
    private int currentChild;
    
    void Start()
    {
        panelLocation = transform.position;
    }

    public void OnDrag(PointerEventData data)
    {
        float difference = data.pressPosition.x - data.position.x;
        transform.position = panelLocation - new Vector3(difference,0,0); 

    }

    public void OnEndDrag(PointerEventData data)
    {
        float percentage = (data .pressPosition.x - data.position.x) / Screen.width;
        if(Mathf.Abs(percentage) > percentThreshold ) 
        {
            Vector3 newLocation = panelLocation;
            if (percentage > 0 && currentChild < transform.childCount - 1)
            {
                newLocation += new Vector3(-Screen.width, 0, 0);
                currentChild++;
            }
            else if (percentage < 0 && currentChild > 0)
            {
                newLocation += new Vector3(Screen.width, 0, 0);
                currentChild--;
            }
            StartCoroutine(SmoothDrag(transform.position, newLocation, Ease));
            panelLocation = newLocation;
        }
        else
        {
            StartCoroutine(SmoothDrag(transform.position, panelLocation, Ease));
        }
    }

    IEnumerator SmoothDrag(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while(t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }
}
