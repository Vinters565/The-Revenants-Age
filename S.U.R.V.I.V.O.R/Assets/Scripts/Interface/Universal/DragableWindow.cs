using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;
using UnityEngine.EventSystems;
public class DragableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform rectTransform;

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / MainCanvas.Instance.Canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
