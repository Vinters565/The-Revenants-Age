using System;
using System.Collections.Generic;
using Inventory.TownInventory;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Economy
{
    [Obsolete]
    public class TradersPanelDrawer : MonoBehaviour
    {
        [SerializeField] private ChooseTraderButtonDrawer traderButtonDrawerPrefab;
        [SerializeField] private LayoutGroup traderButtonsLayoutGroup;
        [SerializeField] private TownInventory tradersInventory;
        [SerializeField] private BuyList buyList;

        private Dictionary<Trader, ChooseTraderButtonDrawer> chooseTraderButtons = new ();
        public void ReDraw(IEnumerable<Trader> traders)
        {
            GenerateNewButtons(traders);
        }

        private void GenerateNewButtons(IEnumerable<Trader> traders)
        {
            foreach (var valuePair in chooseTraderButtons)
            {
                var button = valuePair.Value;
                Destroy(button.gameObject);
            }

            var isTraderChosen = false;
            
            foreach (var trader in traders)
            {
                var instantiatedButton = Instantiate(traderButtonDrawerPrefab.gameObject, traderButtonsLayoutGroup.transform);
                var instantiatedTraderScript = instantiatedButton.GetComponent<ChooseTraderButtonDrawer>();
                instantiatedTraderScript.Init(trader, tradersInventory);
            }
        }
    }
}