using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "New PackedContainerData", menuName = "Data/Packed Container Data", order = 50)]
public class PackedContainerData : ScriptableObject
{
    [SerializeField] private List<InventoryItem> packedItems;

    public IEnumerable<InventoryItem> PackedItems => packedItems;
}
