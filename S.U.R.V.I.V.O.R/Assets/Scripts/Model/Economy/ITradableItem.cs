using System.Collections.Generic;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public interface ITradableItem
    {
        public Currencies ItemCosts { get; }
        public ItemTradeType ItemTradeType { get; }
        public BaseItem BaseItem { get; }
    }
}