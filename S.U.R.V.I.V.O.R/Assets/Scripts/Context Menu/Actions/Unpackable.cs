using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Context_Menu;
using Inventory;
using TheRevenantsAge;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(ContextMenuItem))]
public class Unpackable : MonoBehaviour, IContextMenuAction
{
    public string ButtonText { get; private set; }

    public bool Extendable { get; private set; }

    public AudioClip SoundOnClick { get; }

    private PackedContainer packedContainer;

    private InventoryController inventoryController;

    public void Awake()
    {
        ButtonText = "Распаковать";
        Extendable = false;
        inventoryController = InventoryController.Instance;
        packedContainer = GetComponent<PackedContainer>();
    }

    public void OnButtonClickAction<T>(T value)
    {
        var itemOwner = GetComponent<BaseItem>().ItemOwner;
        var unPackedItems = packedContainer.Unpack();

        foreach (var packed in unPackedItems)
        {
            bool isSuccess;
            if (itemOwner != null)
                isSuccess = itemOwner.ManBody.PlaceItemToInventory(Instantiate(packed));
            else
            {
                inventoryController.ThrowItemAtLocation(Instantiate(packed));
                isSuccess = true;
            }

            if (!isSuccess)
            {
                Debug.Log(
                    $"Вам некуда положить один из предметов, получившихся в результате распаковки, он был уничтожен");
            }
        }
    }
}