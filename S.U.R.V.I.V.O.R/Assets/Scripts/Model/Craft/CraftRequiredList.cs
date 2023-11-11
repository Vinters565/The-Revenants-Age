using System.Collections.Generic;
using Interface.CraftLayerLogic;
using UnityEngine;

namespace TheRevenantsAge
{
    [System.Serializable]
    public class CraftRequiredList : ICraftRequiredItem
    {
        [SerializeField] private List<BaseItem> items;
        [SerializeField] private int amountOfItems;
        [SerializeField] private Sprite sprite;
        public int AmountOfItems => amountOfItems;
        public Sprite Sprite => sprite;

        public List<BaseItem> Items
        {
            get => items;
            set => items = value;
        }

        public T Accept<T>(IRecipeDrawersVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public bool IsItemAcceptedForRequire(BaseItem item)
        {
            return items.Contains(item);
        }

    }
}