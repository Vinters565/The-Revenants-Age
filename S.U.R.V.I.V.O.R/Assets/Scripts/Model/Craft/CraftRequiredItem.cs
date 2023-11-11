using Interface.CraftLayerLogic;
using Mono.Cecil;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    [System.Serializable]
    public class CraftRequiredItem : ICraftRequiredItem
    {
        public BaseItem requiredItem;
        
        public int amountOfItems;
        
        public int AmountOfItems => amountOfItems;

        public Sprite Sprite => requiredItem.Data.Icon;
        public T Accept<T>(IRecipeDrawersVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public bool IsItemAcceptedForRequire(BaseItem item)
        {
            return item.Equals(requiredItem);
        }
    }
}