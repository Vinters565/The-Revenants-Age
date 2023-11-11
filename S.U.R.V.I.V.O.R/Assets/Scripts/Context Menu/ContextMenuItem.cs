using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Context_Menu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ContextMenuItem : MonoBehaviour, IPointerClickHandler
{
    private List<IContextMenuAction> contextMenuActions;

    private void Start()
    {
        contextMenuActions = GetComponents<IContextMenuAction>().ToList();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        ContextMenuController.Instance.Clear();
        ContextMenuController.Instance.CreateContextMenu(contextMenuActions, Input.mousePosition);
    }
}