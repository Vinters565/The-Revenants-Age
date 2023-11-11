using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Inventory.TownInventory;
using UnityEngine;

namespace TheRevenantsAge
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New TraderData", menuName = "Data/Trader Data")]
    public class Trader : ScriptableObject
    {
        [SerializeField] private string traderName;
        [SerializeField] private List<BuyableItem> itemsToSell;
        [SerializeField] private SerializedDictionary<ItemTradeType, float> tradableCoef;
        [SerializeField] private MoneyType moneyType;
        public string TraderName => traderName;

        public MoneyType MoneyType => moneyType;

        public List<BuyableItem> ItemsToSell => itemsToSell;

        public IEnumerable<ItemCost> BuyAndReturnBuyedItems(Group group, IEnumerable<ItemCost> items)
        {
            foreach (var item in items)
            {
                if (tradableCoef.Keys.Contains(item.ItemTradeType))
                {
                    group.Currencies[moneyType] += GetItemCost(item);
                    yield return item;
                }
            }
        }

        public uint GetItemCost(ITradableItem item)
        {
            return (uint)Math.Round(item.ItemCosts[moneyType].Amount * tradableCoef[item.ItemTradeType]);
        }
    }
}