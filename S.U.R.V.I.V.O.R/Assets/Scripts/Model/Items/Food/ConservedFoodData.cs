using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "New ConservedFoodData", menuName = "Data/Conserved Food Data", order = 50)]
public class ConservedFoodData: ScriptableObject
{
    [SerializeField] private InventoryItem itemToSpawnAfterConserveOpen;

    public InventoryItem ItemToSpawnAfterConserveOpen => itemToSpawnAfterConserveOpen;
}
