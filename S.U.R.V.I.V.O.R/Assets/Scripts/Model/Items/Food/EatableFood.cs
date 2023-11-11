using Inventory;
using TheRevenantsAge;
using UnityEngine;

[RequireComponent(typeof(BaseItem))]
[RequireComponent(typeof(Eatable))]
[RequireComponent(typeof(InventoryItem))]
public class EatableFood: MonoBehaviour
{
    [SerializeField] private EatableFoodData data;
    
    public EatableFoodData Data => data;
}