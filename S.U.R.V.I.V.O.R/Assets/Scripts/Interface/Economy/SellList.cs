using System.Linq;
using Inventory.TownInventory;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellList : MonoBehaviour
{
    [SerializeField] private SellInventory sellInventory;
    [SerializeField] private TMP_Text sumOfSellingText;
    [SerializeField] private Button sellButton;

    private int sumOfSelling;

    private MoneyType moneyType => trader.MoneyType;

    private Trader trader;
    
    public int SumOfSelling
    {
        get => sumOfSelling;
        set
        {
            sumOfSelling = value;
            sumOfSellingText.text = value.ToString();
        }
    }

    private void Awake()
    {
        sellButton.onClick.AddListener(Sell);
        sellInventory.itemPlaced += OnItemPlaced;
        sellInventory.itemPickedUp += OnItemPickedUp;
    }

    public void Init(Trader trader)
    {
        this.trader = trader;
        SumOfSelling = 0;
    }

    private void OnItemPickedUp(ITradableItem obj)
    {
        SumOfSelling -= (int)trader.GetItemCost(obj);
    }

    private void OnItemPlaced(ITradableItem obj)
    {
        SumOfSelling += (int)trader.GetItemCost(obj);
    }

    private void Sell()
    {
        var trader = TownWindow.Instance.CurrentTrader;
        var allItems = sellInventory.GetItems().Select(x => x.GetComponent<ItemCost>());
        var itemsToDestroy = trader.BuyAndReturnBuyedItems(GlobalMapController.ChosenGroup,allItems);
        foreach (var itemCost in itemsToDestroy)
        {
            Destroy(itemCost.gameObject);
        }
        SumOfSelling = 0;
    }
}
