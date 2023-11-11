using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public class ItemCost : MonoBehaviour, ITradableItem
    {
        [SerializeField] private SerializedDictionary<MoneyType, int> cost;
        [SerializeField] private ItemTradeType itemTradeType;
        
        private Currencies currencies;
        
        private BaseItem baseItem;
        public ItemTradeType ItemTradeType => itemTradeType;
        public BaseItem BaseItem => baseItem;
        public Currencies ItemCosts => currencies;
        
        private void Awake()
        {
            baseItem = GetComponent<BaseItem>();
            currencies = new Currencies(cost);
        }
    }

    public enum ItemTradeType
    {
        Scrap,
        Medicine,
        Food,
        AmmoContainers,
        GunModules,
        Clothes,
        GunMakeshift,
        Gun
    }
}   