using System.Collections.Generic;
using System.Linq;
using Inventory;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    [RequireComponent(typeof(BaseItem))]
    [RequireComponent(typeof(Salvagable))]
    public class Scrap : MonoBehaviour
    {
        [SerializeField] private List<ItemChance> SalvagableItems;

        private List<InventoryItem> itemChances;
        public void Awake()
        {
            itemChances = new InventoryItem[SalvagableItems.Sum(i => i.WeightChance)].ToList();
            var index = 0;
            foreach (var chance in SalvagableItems)
                for (var i = 0; i < chance.WeightChance; i++)
                {
                    itemChances[index] = chance.Item;
                    index++;
                }
        }

        public IEnumerable<InventoryItem> salvagableItems => itemChances;
    }
}
