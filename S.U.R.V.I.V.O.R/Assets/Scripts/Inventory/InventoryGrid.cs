using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheRevenantsAge;
using UnityEngine;

namespace Inventory
{
    public class InventoryGrid : MonoBehaviour
    {
        public const int REAL_TILE_SIZE = 50;
        public const int TILE_SPACING = -1;
        public static int TileSize => REAL_TILE_SIZE + TILE_SPACING;

        [SerializeField] private Vector2Int initializeSize;
        [SerializeField] private Vector2Int maxSize;

        [SerializeField] private InventoryGridBackground inventoryGridBG;

        private Canvas canvas;
        private InventoryState curInventoryState;

        private bool isNeedToEnableHighlight;

        private Vector2 positionOnGrid;
        private Vector2Int tileGridPosition;

        private RectTransform rectTransform;
        public IGlobalMapCharacter InventoryOwner { get; set; }

        public Vector2Int CurrentSize
        {
            get
            {
                if (curInventoryState == null)
                    return Vector2Int.zero;
                return curInventoryState.Size;
            }
        }

        public Vector2Int MaxSize => maxSize;

        protected void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            rectTransform = GetComponent<RectTransform>();
            curInventoryState = new InventoryState(initializeSize);
            inventoryGridBG.ReDrawBackground();
            rectTransform.sizeDelta = initializeSize * TileSize;
            curInventoryState.PlaceItemEvent += OnPlaceItem;
            curInventoryState.PickUpItemEvent += OnPickedItem;
        }

        private void OnEnable()
        {
            if (Game.IsDebug && isNeedToEnableHighlight)
            {
                InventoryController.Instance.GroupInventoryChanged += inventoryGridBG.DebugDrawGreenSquares;
                inventoryGridBG.DebugDrawGreenSquares();
            }
        }

        private void OnDisable()
        {
            if (Game.IsDebug && isNeedToEnableHighlight)
                InventoryController.Instance.GroupInventoryChanged -= inventoryGridBG.DebugDrawGreenSquares;
        }

        private void Update()
        {
            if (Game.IsDebug)
            {
                if (Input.GetKeyDown(KeyCode.K))
                {
                    isNeedToEnableHighlight = !isNeedToEnableHighlight;
                    inventoryGridBG.DebugDrawGreenSquares();
                }
            }
        }

        public void ChangeState(InventoryState inventoryState)
        {
            if (inventoryState.Size.x > maxSize.x || inventoryState.Size.y > maxSize.y)
                throw new ArgumentException();

            if (curInventoryState != null)
            {
                curInventoryState.PlaceItemEvent -= OnPlaceItem;
                curInventoryState.PickUpItemEvent -= OnPickedItem;
                foreach (var item in curInventoryState.GetItems())
                    item.gameObject.SetActive(false);
            }
            
            GetComponent<RectTransform>().sizeDelta = inventoryState.Size * TileSize;
            curInventoryState = inventoryState;
            curInventoryState.PlaceItemEvent += OnPlaceItem;
            curInventoryState.PickUpItemEvent += OnPickedItem;
            RedrawGrid();
            inventoryGridBG.ReDrawBackground();
        }

        public Vector2Int GetTileGridPosition(Vector2 mousePosition)
        {
            var position = rectTransform.position;
            positionOnGrid.x = mousePosition.x - position.x;
            positionOnGrid.y = position.y - mousePosition.y;

            var scaleFactor = canvas.scaleFactor;
            tileGridPosition.x = (int) ((positionOnGrid.x / TileSize) / scaleFactor);
            tileGridPosition.y = (int) ((positionOnGrid.y / TileSize) / scaleFactor);

            return tileGridPosition;
        }

        public virtual bool PlaceItem(InventoryItem item, int posX, int posY, ref InventoryItem overlapItem)
        {
            var res = curInventoryState.PlaceItem(item, posX, posY, ref overlapItem);
            if (res)
            {
                item.BaseItem.ItemOwner = InventoryOwner;
                var itemRectTransform = item.GetComponent<RectTransform>();
                itemRectTransform.SetParent(rectTransform);

                var position = GetPositionOnGrid(item, posX, posY);
                itemRectTransform.localPosition = position;
            }

            return res;
        }

        public virtual void PlaceItem(InventoryItem item, int posX, int posY)
        {
            item.BaseItem.ItemOwner = InventoryOwner;
            var itemRectTransform = item.GetComponent<RectTransform>();
            itemRectTransform.SetParent(rectTransform);

            var position = GetPositionOnGrid(item, posX, posY);
            itemRectTransform.localPosition = position;


            curInventoryState.PlaceItem(item, posX, posY);
        }

        public Vector2 GetPositionOnGrid(InventoryItem item, int posX, int posY) =>
            new(TileSize * posX + TileSize * item.Width / 2, -(TileSize * posY + TileSize * item.Height / 2));


        public virtual InventoryItem PickUpItem(int x, int y)
        {
            var item = curInventoryState.PickUpItem(x, y);
            if (item is not null)
                item.BaseItem.ItemOwner = null;
            return item;
        }

        public virtual void PickUpItem(InventoryItem item)
        {
            if (item is null)
                return;
            item.BaseItem.ItemOwner = null;
            curInventoryState.PickUpItem(item.OnGridPositionX, item.OnGridPositionY);
        }

        public void ClearWithoutDeletingObjects()
        {
            curInventoryState?.Clear();
        }
        
        public void Clear()
        {
            var items = curInventoryState.GetItems();
            curInventoryState?.Clear();
            foreach (var item in items)
                Destroy(item.gameObject);
        }

        public void Sort(Func<IEnumerable<InventoryItem>, IOrderedEnumerable<InventoryItem>> sortedMethod,
            bool sortDescending)
        {
            if (curInventoryState != null)
                curInventoryState.Sort(sortedMethod, sortDescending);
        }

        public IEnumerable<T> GetItemsFromInventoryByType<T>() where T : MonoBehaviour =>
            curInventoryState.GetItemsFromInventoryByType<T>();

        public bool InsertItem(InventoryItem itemToInsert)
        {
            var foundPosition = curInventoryState.FindSpaceForObject(itemToInsert);
            if (foundPosition == null && this == LocationInventory.Instance.LocationInventoryGrid)
                IncreaseInventorySizeVertically();
            
            return curInventoryState.InsertItem(itemToInsert);;
        }

        public bool PositionCheck(int posX, int posY) => curInventoryState.PositionCheck(posX, posY);

        public bool IsSlotFree(Vector2Int pos) => curInventoryState.IsSlotFree(pos);

        public bool BoundryCheck(int posX, int posY, int width, int height) =>
            curInventoryState.BoundryCheck(posX, posY, width, height);

        public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert) =>
            curInventoryState.FindSpaceForObject(itemToInsert);

        public InventoryItem GetItem(int x, int y) => curInventoryState.GetItem(x, y);

        public IEnumerable<InventoryItem> GetItems() => curInventoryState.GetItems();

        public void ResetSizeToInitialize()
        {
            curInventoryState.ResetSizeToInitialize(initializeSize);
            maxSize = initializeSize;
            inventoryGridBG.ReDrawBackground();
            rectTransform.sizeDelta = initializeSize * TileSize;
            transform.parent.GetComponent<RectTransform>().sizeDelta = initializeSize * TileSize;
        }

        private void RedrawGrid()
        {
            foreach (var item in curInventoryState.GetItems())
            {
                item.gameObject.SetActive(true);
                var position = GetPositionOnGrid(item, item.OnGridPositionX, item.OnGridPositionY);
                item.GetComponent<RectTransform>().SetParent(transform);
                item.GetComponent<RectTransform>().localPosition = position;
            }
        }

        private void OnPlaceItem(InventoryItem placedItem)
        {
            var position = GetPositionOnGrid(placedItem, placedItem.OnGridPositionX, placedItem.OnGridPositionY);
            var rt = placedItem.GetComponent<RectTransform>();
            rt.SetParent(transform);
            rt.localPosition = position;
        }

        private void OnPickedItem(InventoryItem pickedUpItem)
        {
            if (canvas == null || pickedUpItem == null) return;
            pickedUpItem.GetComponent<RectTransform>().SetParent(canvas.transform);
        }

        private void IncreaseInventorySizeVertically()
        {
            curInventoryState.ChangeInventorySize(new Vector2Int(maxSize.x, maxSize.y * 2));
            maxSize = curInventoryState.Size;
            transform.parent.GetComponent<RectTransform>().sizeDelta = maxSize * TileSize;
            rectTransform.sizeDelta = maxSize * TileSize;
            inventoryGridBG.ReDrawBackground();
        }
    }
}