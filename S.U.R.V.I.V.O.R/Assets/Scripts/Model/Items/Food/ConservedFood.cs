using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

public class ConservedFood : MonoBehaviour
{
    [SerializeField] private ConservedFoodData data;

    public InventoryItem Open()
    {
        Destroy(gameObject);
        return data.ItemToSpawnAfterConserveOpen;
    }
}
