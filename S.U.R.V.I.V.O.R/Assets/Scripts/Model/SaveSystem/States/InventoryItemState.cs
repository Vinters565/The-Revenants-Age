using System.Runtime.Serialization;
using UnityEngine;

namespace TheRevenantsAge
{
    public class InventoryItemState: ComponentState
    {
        [DataMember] public Vector2Int positionInInventory;
        [DataMember] public bool isRotated;
    }
}