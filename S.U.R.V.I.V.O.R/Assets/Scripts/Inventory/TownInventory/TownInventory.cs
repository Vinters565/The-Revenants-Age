using System.Collections.Generic;
using UnityEngine;

namespace Inventory.TownInventory
{
    public class TownInventory : MonoBehaviour
    {
        [SerializeField] private InventoryGrid inventoryGrid;

        private Dictionary<InventoryItem, BuyableItem> tradableItems = new ();

        public void InsertItem(BuyableItem buyableItem)
        {
            var itemToInsert = buyableItem.InventoryItem;

            var positionOnGrid = FindSpaceForObject(itemToInsert);
            if (positionOnGrid == null)
            {
                return;
            }

            itemToInsert.OnGridPositionX = positionOnGrid.Value.x;
            itemToInsert.OnGridPositionY = positionOnGrid.Value.y;
            inventoryGrid.PlaceItem(itemToInsert, positionOnGrid.Value.x, positionOnGrid.Value.y);
            tradableItems.Add(itemToInsert, buyableItem);
            itemToInsert.Image.raycastTarget = false;
        }

        public void Clear()
        {
            var items = inventoryGrid.GetItems();
            foreach (var inventoryItem in items)
            {
                tradableItems[inventoryItem].gameObject.SetActive(false);
                tradableItems.Remove(inventoryItem);
            }
            inventoryGrid.ClearWithoutDeletingObjects();
        }
        
        private Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
        {
            var height = inventoryGrid.CurrentSize.y - itemToInsert.Height + 1;
            var width = inventoryGrid.CurrentSize.x - itemToInsert.Width + 1;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (CheckAvailableSpace(x, y, itemToInsert.Width + 2, itemToInsert.Height))
                        return new Vector2Int(x, y);
                }
            }

            return null;
        }
        
        private bool CheckAvailableSpace(int posX, int posY, int width, int height)
        {
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (!IsSlotFree(posX + x, posY + y))
                    return false;
            return true;
        }

        private bool IsSlotFree(int x, int y)
        {
            bool isSlotsFree = (ExtensionCheck(x-1,y) && ExtensionCheck(x-2, y));

            var position = new Vector2Int(x, y);
            return isSlotsFree && x < inventoryGrid.CurrentSize.x && inventoryGrid.IsSlotFree(position);
        }

        private bool ExtensionCheck(int x,int y)
        {
            if (x < 0) return true;
            return  inventoryGrid.IsSlotFree(new Vector2Int(x, y));
        }
    }
}