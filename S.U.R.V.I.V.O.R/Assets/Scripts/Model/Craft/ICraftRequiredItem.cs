using Interface.CraftLayerLogic;
using UnityEngine;

namespace TheRevenantsAge
{
    public interface ICraftRequiredItem
    {
        public bool IsItemAcceptedForRequire(BaseItem item);

        public int AmountOfItems { get; }
        
        public Sprite Sprite { get; }
        
        public T Accept<T>(IRecipeDrawersVisitor<T> visitor);
    }
}