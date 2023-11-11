using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Context_Menu;
using Inventory;
using TheRevenantsAge;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(ContextMenuItem))]
public class Salvagable : MonoBehaviour, IContextMenuAction
{
    public string ButtonText { get; private set; }

    public bool Extendable { get; private set; }

    public AudioClip SoundOnClick { get; }

    private Scrap scrap;

    private InventoryController inventoryController;

    public void Awake()
    {
        ButtonText = "Разобрать";
        Extendable = false;
        inventoryController = InventoryController.Instance;
        scrap = GetComponent<Scrap>();
    }

    public void OnButtonClickAction<T>(T value)
    {
        var itemOwner = GetComponent<BaseItem>().ItemOwner;
        var salvagedItems = scrap.salvagableItems;
        var clothes = gameObject.GetComponent<Clothes>();
        var items = salvagedItems;
        foreach (var item in items)
        {
            bool isSuccess;
            if (itemOwner != null)
                isSuccess = itemOwner.ManBody.PlaceItemToInventory(Instantiate(item));
            else
            {
                inventoryController.ThrowItemAtLocation(Instantiate(item));
                isSuccess = true;
            }

            if (!isSuccess)
            {
                Debug.Log(
                    $"Вам некуда положить один из предметов, получившихся в результате распаковки, он был уничтожен"); //TODO При расширяемом инвентаре локации убрать
            }
        }

        Destroy(clothes);
        Destroy(gameObject);
    }
}