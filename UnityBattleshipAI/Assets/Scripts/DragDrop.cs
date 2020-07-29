using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    /*/ Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    private RectTransform rectTransform;

    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();

    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        Debug.Log("Start");

    }

    public void OnDrag(PointerEventData eventData)
    {

        Debug.Log("Dragging");
        rectTransform.anchoredPosition += eventData.delta;


    }

    public void OnPointerDown(PointerEventData eventData)
    {

        Debug.Log("Click");

    }

    public void OnEndDrag(PointerEventData eventData)
    {

        Debug.Log("Stop");

    }
}
