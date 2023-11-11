using Inventory;
using UnityEngine;

namespace TheRevenantsAge
{
    [System.Serializable]
    public class ItemChance
    {
        [SerializeField] [HideInInspector] private string title;
    
        [SerializeField] private InventoryItem item;
        [SerializeField] private int weightChance = 1;

        public InventoryItem Item => item;
        public int WeightChance => weightChance;

        public void Validate()
        {
            if (item is null)
            {
                title = "No Item";
            }
            else if (item == null)
            {
                title = "Missing Item";
            }
            else
            {
                title = $"{item.name} x{weightChance}";
            }
        }
    }
}