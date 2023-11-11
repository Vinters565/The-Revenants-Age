using System;
using System.Collections.Generic;
using System.Linq;
using Interface.Economy;
using Interface.Universal;
using Inventory;
using Inventory.TownInventory;
using UnityEngine;
using UnityEngine.UI;

namespace TheRevenantsAge
{
    public class TradersPanelLogic : Switch
    {
        [SerializeField] private ChooseTraderButtonLogic chooseTraderButtonLogicPrefab;
        [SerializeField] private LayoutGroup traderButtonsLayoutGroup;
        [SerializeField] private TownInventory tradersInventory;
        [SerializeField] private SellList sellList;

        private Trader chosenTrader;

        private Dictionary<int, TraderObject> tradersDict;

        private readonly List<ChooseTraderButtonLogic> chooseTraderButtonsList = new ();

        public Trader ChosenTrader => chosenTrader;

        public SellList SellList => sellList;

        public void ReDraw(List<Trader> traders)
        {
            GenerateNewButtons(traders);
        }

        private void GenerateNewButtons(List<Trader> traders)
        {
            foreach (var button in chooseTraderButtonsList)
            {
                Destroy(button.gameObject);
            }

            tradersDict = new Dictionary<int, TraderObject>();

            for (int i = 0; i < traders.Count(); i++)
            {
                var trader = traders[i];
                tradersDict.Add(i, new TraderObject(trader));
                var instantiatedButton = Instantiate(chooseTraderButtonLogicPrefab.gameObject, traderButtonsLayoutGroup.transform);
                var instantiatedTraderScript = instantiatedButton.GetComponent<ChooseTraderButtonLogic>();
                instantiatedTraderScript.InitReference(i, this, trader);
                AddButton(instantiatedTraderScript);
            }
        }

        private void AddButton(ChooseTraderButtonLogic button)
        {
            base.AddButton(button);
            chooseTraderButtonsList.Add(button);
        }

        public void OnButtonClick(int ButtonId)
        {
            var traderToReDrawInterface = tradersDict[ButtonId];
            chosenTrader = traderToReDrawInterface.Trader;
            Debug.Log(chosenTrader.TraderName);
            ReDrawInterface(traderToReDrawInterface);
        }

        private void ReDrawInterface(TraderObject trader)
        {
            tradersInventory.Clear();
            SellList.Init(chosenTrader);
            foreach (var item in trader.InstantiatedItems)
            {
                item.gameObject.SetActive(true);
                tradersInventory.InsertItem(item);
                item.InventoryItem.Backgroung.rectTransform.sizeDelta = item.InventoryItem.Image.rectTransform.sizeDelta;
                item.transform.SetParent(tradersInventory.transform);
                item.transform.position = item.InventoryItem.transform.position;
                item.InventoryItem.transform.SetParent(item.transform);
            }
        }
    }
}