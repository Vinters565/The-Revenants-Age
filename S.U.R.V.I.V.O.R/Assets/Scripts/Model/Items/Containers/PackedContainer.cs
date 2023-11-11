using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

public class PackedContainer : MonoBehaviour
{
    [SerializeField] private PackedContainerData data;

    public IEnumerable<InventoryItem> ShowUnpackedItemsTypes()
    {
        return data.PackedItems;
    }
    public IEnumerable<InventoryItem> Unpack()
    {
        Destroy(gameObject);
        return data.PackedItems;
    }
}
