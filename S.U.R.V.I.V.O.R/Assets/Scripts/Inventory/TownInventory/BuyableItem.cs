using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Inventory.TownInventory
{
    public class BuyableItem : MonoBehaviour
    {
        [FormerlySerializedAs("inventoryItem")] [SerializeField] private InventoryItem inventoryItem;
        [SerializeField] private MoneyType moneyType;
        [SerializeField] private uint cost;
        [SerializeField] private Button button;

        public InventoryItem InventoryItem => inventoryItem;

        public MoneyType MoneyType => moneyType;

        public uint Cost => cost;

        public string CurrencySymbol => moneyType == MoneyType.CenterRubles ? "Р" : "$";
        
        private RectTransform rectTransform;

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            TownWindow.Instance.BuyList.AddElement(this);
        }
    }
}