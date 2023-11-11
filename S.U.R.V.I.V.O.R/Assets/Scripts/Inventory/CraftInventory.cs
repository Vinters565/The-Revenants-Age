using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Inventory
{
    public class CraftInventory : MonoBehaviour
    {
        public static CraftInventory Instance { get; private set; }

        [SerializeField]
        private InventoryGrid inventoryGrid;

        public InventoryGrid CraftInventoryGrid => inventoryGrid;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
                Destroy(gameObject);
        }
        
        public static IEnumerable<T> GetItemsFromInventoryByType<T>() where T : MonoBehaviour =>
            Instance.CraftInventoryGrid.GetItemsFromInventoryByType<T>();
    }
}
