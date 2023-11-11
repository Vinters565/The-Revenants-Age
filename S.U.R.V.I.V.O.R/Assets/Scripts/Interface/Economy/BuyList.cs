using System;
using System.Collections.Generic;
using Inventory;
using Inventory.TownInventory;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Economy
{
    public class BuyList : MonoBehaviour
    {
        [SerializeField] private LayoutGroup elementsLayoutGroup;
        [SerializeField] private TradeElementDrawer tradeElementDrawerPrefab;
        [SerializeField] private List<TradeElementDrawer> tradeElementDrawers;
        [SerializeField] private TMP_Text summText;
        
        [SerializeField] private InventoryGrid boughtItemsInventoryGrid;
        [SerializeField] private Button buyButton;
        
        private Dictionary<MoneyType, uint> moneyAmount;

        private void Awake()
        {
            moneyAmount = new Dictionary<MoneyType, uint>
            { 
                {MoneyType.CenterRubles, 0}, 
                {MoneyType.PortDollar, 0}  
            };
            ReDrawSummText();
            buyButton.onClick.AddListener(Buy);
        }

        public void AddElement(BuyableItem buyableItem)
        {
            var tradeElement = Instantiate(tradeElementDrawerPrefab, elementsLayoutGroup.transform);
            tradeElement.summChanged.AddListener(OnSummChanged);
            tradeElement.BuyableItem = buyableItem;
            tradeElementDrawers.Add(tradeElement);
        }

        private void OnSummChanged(MoneyType moneyType, uint oldSumm, uint newSumm)
        {
            moneyAmount[moneyType] += newSumm - oldSumm;
            ReDrawSummText();
        }

        private void ReDrawSummText()
        {
            summText.text = $"$ {moneyAmount[MoneyType.PortDollar]} & Р {moneyAmount[MoneyType.CenterRubles]}";
        }

        private void Buy()
        {
            foreach (var tradeElementDrawer in tradeElementDrawers)
            {
                for (int i = 0; i < tradeElementDrawer.AmountOfItems; i++)
                {
                    var instantiatedItem = Instantiate(tradeElementDrawer.BuyableItem.InventoryItem.gameObject);

                    var group = GlobalMapController.ChosenGroup;

                    group.Currencies[tradeElementDrawer.BuyableItem.MoneyType] -= tradeElementDrawer.BuyableItem.Cost;
                
                    boughtItemsInventoryGrid.InsertItem(instantiatedItem.GetComponent<InventoryItem>());
                }
                tradeElementDrawer.Destroy();
            }
            tradeElementDrawers.Clear();
        }
    }       
}