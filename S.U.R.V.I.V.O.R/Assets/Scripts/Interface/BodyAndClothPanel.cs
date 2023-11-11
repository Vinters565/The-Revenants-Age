using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using TheRevenantsAge;
using UnityEngine;

public class BodyAndClothPanel : MonoBehaviour
{
    [SerializeField] private GameObject clothPanel;
    [SerializeField] private GameObject bodyIndicatorPanel;

    
    void Start()
    {
        InventoryController.Instance.SelectedItemChanged += OnInventoryControllerSelectedItemChanged;
    }

    private void OnInventoryControllerSelectedItemChanged(InventoryItem item)
    {
        if (item == null)
        {
            //SwapToBodyIndicatorPanel();
            return;
        }
        var cloth = item.GetComponent<Clothes>();
        if (cloth != null)
        {
            SwapToClothPanel();
        } 
    }
    
    private void SwapToClothPanel()
    {
        clothPanel.SetActive(true);
        bodyIndicatorPanel.SetActive(false);
    }

    private void SwapToBodyIndicatorPanel()
    {
        clothPanel.SetActive(false);
        bodyIndicatorPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        InventoryController.Instance.SelectedItemChanged -= OnInventoryControllerSelectedItemChanged;
    }
}
