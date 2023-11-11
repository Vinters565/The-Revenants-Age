using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryState
    {
        private Vector2Int size;
        private InventoryItem[,] inventoryItemSlot;

        public Vector2Int Size => size;

        public event Action InventoryChange;
        public event Action<InventoryItem> PlaceItemEvent;
        public event Action<InventoryItem> PickUpItemEvent;

        public InventoryState(Vector2Int size)
        {
            this.size = size;
            inventoryItemSlot = new InventoryItem[size.x, size.y];
        }

        public IEnumerable<InventoryItem> GetItems()
        {
            var array = new InventoryItem[inventoryItemSlot.GetLength(0) * inventoryItemSlot.GetLength(1)];
            var i = 0;
            foreach (var item in inventoryItemSlot)
            {
                array[i] = item;
                i++;
            }

            return array.Distinct().Where(x => x != null);
        }

        public InventoryItem PickUpItem(int x, int y)
        {
            var returnedItem = inventoryItemSlot[x, y];
            if (returnedItem == null) return null;
            RemoveGridReference(returnedItem);
            inventoryItemSlot[x, y] = null;
            PickUpItemEvent?.Invoke(returnedItem);
            InventoryChange?.Invoke();
            returnedItem.Destroed -= (OnDestroyEvent);
            return returnedItem;
        }

        public bool PlaceItem(InventoryItem item, int posX, int posY, ref InventoryItem overlapItem)
        {
            if (!BoundryCheck(posX, posY, item.Width, item.Height))
                return false;

            if (!OverlapCheck(posX, posY, item.Width, item.Height, ref overlapItem))
            {
                overlapItem = null;
                return false;
            }

            if (overlapItem != null)
            {
                RemoveGridReference(overlapItem);
                PickUpItemEvent?.Invoke(overlapItem);
            }

            PlaceItem(item, posX, posY);
            return true;
        }

        public void PlaceItem(InventoryItem item, int posX, int posY)
        {
            if (!BoundryCheck(posX, posY, item.Width, item.Height))
                return;
            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    inventoryItemSlot[posX + x, posY + y] = item;
                }
            }

            item.Image.raycastTarget = true;

            item.OnGridPositionX = posX;
            item.OnGridPositionY = posY;
            PlaceItemEvent?.Invoke(item);
            item.Destroed += (OnDestroyEvent);
            InventoryChange?.Invoke();
        }

        public bool PositionCheck(int posX, int posY) => posX >= 0 && posY >= 0 && posX < size.x && posY < size.y;

        public bool BoundryCheck(int posX, int posY, int width, int height) =>
            PositionCheck(posX, posY) && PositionCheck(posX + width - 1, posY + height - 1);

        public InventoryItem GetItem(int x, int y) => inventoryItemSlot[x, y];

        public bool InsertItem(InventoryItem itemToInsert)
        {
            var positionOnGrid = FindSpaceForObject(itemToInsert);
            if (positionOnGrid == null)
            {
                return false;
            }

            itemToInsert.OnGridPositionX = positionOnGrid.Value.x;
            itemToInsert.OnGridPositionY = positionOnGrid.Value.y;
            PlaceItem(itemToInsert, positionOnGrid.Value.x, positionOnGrid.Value.y);
            return true;
        }

        public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
        {
            var height = size.y - itemToInsert.Height + 1;
            var width = size.x - itemToInsert.Width + 1;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (CheckAvailableSpace(x, y, itemToInsert.Width, itemToInsert.Height))
                        return new Vector2Int(x, y);
                }
            }

            return null;
        }

        public bool IsSlotFree(Vector2Int pos)
        {
            return inventoryItemSlot[pos.x, pos.y] == null;
        }

        public void Clear()
        {
            var items = GetItems();

            foreach (var item in items)
            {
                item.Destroed -= (OnDestroyEvent);
            }
            
            for (var i = 0; i < inventoryItemSlot.GetLength(0); i++)
            for (int j = 0; j < inventoryItemSlot.GetLength(1); j++)
                inventoryItemSlot[i, j] = null;
        }

        public void Sort(Func<IEnumerable<InventoryItem>, IOrderedEnumerable<InventoryItem>> sortedMethod,
            bool sortDescending)
        {
            IEnumerable<InventoryItem> sortedItems = sortedMethod(GetItems()).ThenBy(item => item.Data.ItemName);
            Clear();
            if (sortDescending)
                sortedItems = sortedItems.Reverse();
            foreach (var item in sortedItems)
            {
                InsertItem(item);
            }
        }

        public IEnumerable<T> GetItemsFromInventoryByType<T>()
            where T : MonoBehaviour
        {
            var result = new List<T>();
            result.AddRange(GetItems()
                .Where(x => x.GetComponent<T>())
                .Select(x => x.GetComponent<T>()));

            return result;
        }

        public void ChangeInventorySize(Vector2Int newSize)
        {
            if (newSize.x < size.x && newSize.y < size.y) return;

            var newInventoryItemSlot = new InventoryItem[newSize.x, newSize.y];
            for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                newInventoryItemSlot[x, y] = inventoryItemSlot[x, y];

            size = newSize;
            inventoryItemSlot = newInventoryItemSlot;
        }

        public void ResetSizeToInitialize(Vector2Int initializeSize)
        {
            inventoryItemSlot = new InventoryItem[initializeSize.x, initializeSize.y];
            size = initializeSize;
        }

        private void OnDestroyEvent(InventoryItem item)
        {
            if (item == null) return;
            RemoveGridReference(item);
        }

        private void RemoveGridReference(InventoryItem item)
        {
            for (int ix = 0; ix < item.Width; ix++)
            {
                for (int iy = 0; iy < item.Height; iy++)
                {
                    inventoryItemSlot[item.OnGridPositionX + ix, item.OnGridPositionY + iy] = null;
                }
            }
        }

        private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (inventoryItemSlot[posX + x, posY + y] != null)
                    {
                        if (overlapItem == null)
                            overlapItem = inventoryItemSlot[posX + x, posY + y];
                        else
                        {
                            if (overlapItem != inventoryItemSlot[posX + x, posY + y])
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool CheckAvailableSpace(int posX, int posY, int width, int height)
        {
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (inventoryItemSlot[posX + x, posY + y] != null)
                    return false;

            return true;
        }
    }
}