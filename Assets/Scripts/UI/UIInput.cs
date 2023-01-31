using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInput : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    InputController controller;

    bool dragging = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        controller.OnDrag(eventData.pressPosition, true);
        dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        controller.OnDrag(eventData.position, false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dragging) return;

        controller.OnClick(eventData.position);
    }
}
