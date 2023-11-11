using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

[DisallowMultipleComponent]
public class CookableFood : EatableFood
{
    [SerializeField] private List<InventoryItem> objectToSpawnAfterCook;

    public IEnumerable<InventoryItem> ObjectToSpawnAfterCook => objectToSpawnAfterCook;

    public IEnumerable<InventoryItem> Cook()
    {
        Destroy(gameObject);
        return objectToSpawnAfterCook;
    }
}
