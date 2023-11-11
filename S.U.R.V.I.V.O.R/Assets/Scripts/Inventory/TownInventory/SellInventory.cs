using System;
using TheRevenantsAge;

namespace Inventory.TownInventory
{
    public class SellInventory : InventoryGrid
    {
        public event Action<ITradableItem> itemPlaced;
        public event Action<ITradableItem> itemPickedUp;

        public override bool PlaceItem(InventoryItem item, int posX, int posY, ref InventoryItem overlapItem)
        {
            var tradableItem = item.GetComponent<ITradableItem>();
            if (tradableItem == null) return false;
            var res = base.PlaceItem(item, posX, posY, ref overlapItem);
            if (res)
            {
                itemPlaced?.Invoke(tradableItem);
            }
            return res;
        }

        public override void PlaceItem(InventoryItem item, int posX, int posY)
        {
            base.PlaceItem(item, posX, posY);
            itemPlaced?.Invoke(item.GetComponent<ITradableItem>());
        }

        public override InventoryItem PickUpItem(int x, int y)
        {
            var res =  base.PickUpItem(x, y);
            var tradableItem = res?.GetComponent<ITradableItem>();
            if(tradableItem != null)
                itemPickedUp?.Invoke(tradableItem);
            return res;
        }

        public override void PickUpItem(InventoryItem item)
        {
            base.PickUpItem(item);
            var tradableItem = item.GetComponent<ITradableItem>();
            if(tradableItem != null)
            {
                itemPickedUp?.Invoke(tradableItem);
            }
        }
    }
}