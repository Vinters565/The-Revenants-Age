using System;
using System.Collections.Generic;
using Inventory.TownInventory;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheRevenantsAge
{
    public class TraderObject
    {
        private Trader trader;
        
        private List<BuyableItem> instantiatedItems = new ();

        public IEnumerable<BuyableItem> InstantiatedItems => instantiatedItems;

        public Trader Trader => trader;

        public TraderObject(Trader trader)
        {
            this.trader = trader;
            InstantiateAllTradableItems();
        }

        private void InstantiateAllTradableItems()
        {
            foreach (var item in trader.ItemsToSell)
            {
                var instance = Object.Instantiate(item.gameObject).GetComponent<BuyableItem>();
                instantiatedItems.Add(instance);
                instance.gameObject.SetActive(false);
            }
        }
    }
}