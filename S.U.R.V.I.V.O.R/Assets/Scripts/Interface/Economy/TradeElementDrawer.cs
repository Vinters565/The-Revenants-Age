using System;
using Inventory.SpecialCells;
using Inventory.TownInventory;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TradeElementDrawer : MonoBehaviour
{
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_InputField amountOfItems;

    private BuyableItem buyableItem;

    private uint cost;

    public UnityEvent<MoneyType, uint, uint> summChanged;

    public int AmountOfItems => (int)(cost / buyableItem.Cost);
    
    public BuyableItem BuyableItem
    {
        get => buyableItem;
        set
        {
            buyableItem = value;
            Init();
        }
    }

    private uint Cost
    {
        get => cost;
        set
        {
            summChanged.Invoke(buyableItem.MoneyType,cost,value);
            cost = value;
        }
    }
    
    private void Init()
    {
        amountOfItems.onValueChanged.AddListener(OnAmountChanged);
        ReDrawImage(buyableItem.InventoryItem.BaseItem.Data.Icon);
        Cost = buyableItem.Cost;
        costText.text = buyableItem.CurrencySymbol + cost;
    }

    private void ReDrawImage(Sprite sprite)
    {
        itemImage.sprite = sprite;
        SpecialCell.ChangeItemWidthHeight(itemImage.rectTransform,itemImage.rectTransform.rect.width, itemImage.rectTransform.rect.height);
    }
    
    private void OnAmountChanged(string arg0)
    {
        if(!int.TryParse(arg0, out var result))
            return;
        var amount = Math.Abs(result);
        amountOfItems.text = amount.ToString();
        Cost = (uint)amount * buyableItem.Cost;
        costText.text = buyableItem.CurrencySymbol + cost;
        
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        summChanged.Invoke(buyableItem.MoneyType,cost,0);
    }
}
