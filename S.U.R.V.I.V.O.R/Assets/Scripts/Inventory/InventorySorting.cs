using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventorySorting : MonoBehaviour
{
    [SerializeField] private InventoryGrid inventoryGrid;
    [SerializeField] private TMP_Dropdown sortingMethodMenu;
    [SerializeField] private Toggle sortOrderToggle;
    [SerializeField] private Button applyButton;

    private bool sortDescending;

    private readonly List<Func<IEnumerable<InventoryItem>, IOrderedEnumerable<InventoryItem>>> sortingMethods = new()
    {
        list => list.OrderBy(item => item.Data.Size.x * item.Data.Size.y),
        list => list.OrderBy(item => item.Data.Weight),
        list => list.OrderBy(item => item.Data.ItemName),
    };
    
    private readonly List<string> sortingMethodsDisplayText = new(new[] {"По размеру", "По весу", "По названию"});

    private void Start()
    {
        sortOrderToggle.isOn = false;
        sortingMethodMenu.options =
            new List<TMP_Dropdown.OptionData>(sortingMethodsDisplayText
                .Select(text => new TMP_Dropdown.OptionData(text)).ToList());
        applyButton.onClick.AddListener(Sort);
        sortOrderToggle.onValueChanged.AddListener((value => sortDescending = value));
    }

    private void Sort()
    {
        inventoryGrid.Sort(sortingMethods[sortingMethodMenu.value], sortDescending);
    }
}