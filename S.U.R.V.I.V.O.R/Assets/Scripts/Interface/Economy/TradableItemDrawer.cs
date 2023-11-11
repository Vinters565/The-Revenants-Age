using System;
using Inventory.TownInventory;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interface.Economy
{
    public class TradableItemDrawer : MonoBehaviour
    {
        [FormerlySerializedAs("tradableItem")] [SerializeField] private BuyableItem buyableItem;

        [SerializeField] private TMP_Text priceText;
        private void Awake()
        {
            var charOfCurrency = "$ ";
            if (buyableItem.MoneyType == MoneyType.CenterRubles)
                charOfCurrency = "Р ";
            priceText.text = charOfCurrency + buyableItem.Cost;
        }
    }
}