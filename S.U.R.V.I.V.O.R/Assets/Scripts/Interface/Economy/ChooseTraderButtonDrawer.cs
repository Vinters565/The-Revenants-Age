using System;
using System.Collections.Generic;
using Inventory.TownInventory;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Obsolete]
public class ChooseTraderButtonDrawer : MonoBehaviour
{
    [SerializeField] private TMP_Text traderNameText;

    private List<BuyableItem> instantiatedItems = new ();

    private Trader trader;
    private TownInventory townInventory;
    private Button button;
    
    public void Init(Trader trader, TownInventory townInventory)
    {
        traderNameText.text = trader.TraderName;
        this.trader = trader;
        this.townInventory = townInventory;
        button = GetComponent<Button>();
        InstantiateAllTradableItems();
        button.onClick.AddListener(OnButtonClick);
    }

    private void InstantiateAllTradableItems()
    {
        foreach (var item in trader.ItemsToSell)
        {
            var instance = Instantiate(item.gameObject).GetComponent<BuyableItem>();
            instantiatedItems.Add(instance);
            instance.gameObject.SetActive(false);
        }
    }
    
    private void OnButtonClick()
    {
        townInventory.Clear();
        foreach (var item in instantiatedItems)
        {
            item.gameObject.SetActive(true);
            townInventory.InsertItem(item);
            item.InventoryItem.Backgroung.rectTransform.sizeDelta = item.InventoryItem.Image.rectTransform.sizeDelta;
            item.transform.SetParent(townInventory.transform);
            item.transform.position = item.InventoryItem.transform.position;
            item.InventoryItem.transform.SetParent(item.transform);
        }
    }
}
