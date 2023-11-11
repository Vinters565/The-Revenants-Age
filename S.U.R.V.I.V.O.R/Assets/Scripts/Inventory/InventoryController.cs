using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using TheRevenantsAge;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        public static InventoryController Instance { get; private set; }

        [SerializeField] private Transform canvasTransform;

        private RectTransform rectTransform;
        private InventoryGrid selectedInventoryGrid;

        private InventoryItem selectedItem;
        private InventoryItem overlapItem;
        private InventoryItem itemToHighlight;

        private InventoryInputSystem inventoryActions;
        private InputAction fastMovingItemsAction;
        private InputAction fastMovingItemsSameTypeAction;
        private InputAction rotateItemsAction;
        private InputAction leftClickAction;

        private InventoryHighlight inventoryHighlight;
        private Vector2Int? previousPosition;

        [HideInInspector] public bool isPointerUnderInventory;

        public event Action<InventoryItem> SelectedItemChanged;

        public event Action GroupInventoryChanged;

        public InventoryGrid SelectedInventoryGrid
        {
            get => selectedInventoryGrid;
            set
            {
                selectedInventoryGrid = value;
                inventoryHighlight.SetParent(value);
            }
        }

        public InventoryItem SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem != null) //Вещь на курсоре есть
                {
                    switch (value)
                    {
                        case null: //Кладем вещь с курсора в ячейку
                            selectedItem.Image.raycastTarget = true;
                            break;
                        case not null: //Меняем вещи местами
                            selectedItem.Image.raycastTarget = true;
                            value.Image.raycastTarget = false;
                            break;
                    }
                }
                else
                {
                    switch (value) //Вещи на курсоре нет
                    {
                        case null: //Ничего не происходит
                            break;
                        case not null: //Берем вещь из ячейки
                            value.Image.raycastTarget = false;
                            break;
                    }
                }

                selectedItem = value;
                SelectedItemChanged?.Invoke(selectedItem);
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                Init();
                inventoryActions = new InventoryInputSystem();
            }
        }

        private void OnEnable()
        {
            leftClickAction = inventoryActions.Inventory.LeftButton;
            fastMovingItemsAction = inventoryActions.Inventory.FastMoving;
            fastMovingItemsSameTypeAction = inventoryActions.Inventory.FastMovingItemsSameType;
            rotateItemsAction = inventoryActions.Inventory.RotateItem;
            inventoryActions.Inventory.Enable();
        }

        private void OnDisable()
        {
            if (SelectedItem != null)
            {
                LocationInventory.Instance.LocationInventoryGrid.InsertItem(SelectedItem);
                SelectedItem = null;
                isPointerUnderInventory = false;
            }

            inventoryActions.Inventory.Disable();
        }

        private void Update()
        {
            if (SelectedItem != null)
                rectTransform.position = Input.mousePosition;

            if (rotateItemsAction.triggered)
            {
                RotateItem();
            }

            if (leftClickAction.triggered && selectedItem && !isPointerUnderInventory)
            {
                var ejectedItem = SelectedItem;
                SelectedItem = null;
                ThrowItemAtLocation(ejectedItem);
            }

            if (selectedInventoryGrid == null)
            {
                inventoryHighlight.Show(false);
                return;
            }

            if (leftClickAction.triggered && isPointerUnderInventory)
            {
                var mousePosition = Input.mousePosition;
                var tileGridPosition = GetTileGridPosition(mousePosition);

                if (fastMovingItemsAction.triggered && SelectedItem == null)
                {
                    FastMovingItem(tileGridPosition);
                }
                else if (fastMovingItemsSameTypeAction.triggered && SelectedItem == null)
                {
                    FastMovingItemsSameType(tileGridPosition);
                }
                else
                {
                    if (SelectedItem == null)
                    {
                        PickUpItem(tileGridPosition);
                    }
                    else
                    {
                        PlaceItem(tileGridPosition);
                    }
                }
            }
            
            HandleHighlight();
            inventoryHighlight.transform.SetParent(inventoryHighlight.transform.parent);
        }

        public void PickUpItemFromSpecialCell(InventoryItem item)
        {
            SelectedItem = item;
            ChangeRectTransform();
            GroupInventoryChanged?.Invoke();
        }

        // Перемещение предмета в инвентарь локации при нажатии на пустое пространство
        public void ThrowItemAtLocation(InventoryItem item)
        {
            LocationInventory.Instance.LocationInventoryGrid.InsertItem(item);
        }

        public void AddItemToInventory(InventoryItem item, bool withItemCreation = true)
        {
            if (SelectedItem != null) return;
            var itemToInsert = item;
            if (withItemCreation)
            {
                CreateItem(item);
                itemToInsert = SelectedItem;
                SelectedItem = null;
            }

            InsertItem(itemToInsert);
            selectedInventoryGrid = null;
        }
        
        private void Init()
        {
            inventoryHighlight = GetComponent<InventoryHighlight>();
        }

        private void ChangeRectTransform()
        {
            if (SelectedItem == null) return;
            rectTransform = SelectedItem.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasTransform);
        }

        private void PickUpItem(Vector2Int tileGridPosition, bool playSound = true)
        {
            SelectedItem = selectedInventoryGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
            ChangeRectTransform();
            GroupInventoryChanged?.Invoke();
            if (playSound && SelectedItem != null)
                AudioManager.Instance.PlaySFX(SelectedItem.OnPickUpAudioClip);
        }

        private void PlaceItem(Vector2Int tileGridPosition, bool playSound = true)
        {
            var complete = selectedInventoryGrid.PlaceItem(SelectedItem, tileGridPosition.x, tileGridPosition.y,
                ref overlapItem);
            if (complete)
            {
                if (playSound && SelectedItem != null)
                    AudioManager.Instance.PlaySFX(SelectedItem.OnPlaceAudioClip);
                SelectedItem = null;
                if (overlapItem != null)
                {
                    SelectedItem = overlapItem;
                    overlapItem = null;
                    rectTransform = SelectedItem.GetComponent<RectTransform>();
                    rectTransform.SetAsLastSibling();
                }
            }

            GroupInventoryChanged?.Invoke();
        }

        private void RotateItem()
        {
            if (SelectedItem == null) return;
            SelectedItem.Rotate();
        }

        private void HandleHighlight()
        {
            var mousePosition = Input.mousePosition;
            var positionOnGrid = GetTileGridPosition(mousePosition);
            if (!selectedInventoryGrid.PositionCheck(positionOnGrid.x, positionOnGrid.y))
                return;
            if (previousPosition == positionOnGrid) return;
            if (SelectedItem == null)
            {
                itemToHighlight = selectedInventoryGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
                if (itemToHighlight != null)
                {
                    inventoryHighlight.Show(true);
                    inventoryHighlight.SetSize(itemToHighlight);
                    inventoryHighlight.SetPosition(selectedInventoryGrid, itemToHighlight);
                    inventoryHighlight.transform.SetAsLastSibling();
                    inventoryHighlight.SetParent(selectedInventoryGrid);
                }
                else
                {
                    inventoryHighlight.Show(false);
                }
            }
            else
            {
                inventoryHighlight.Show(selectedInventoryGrid.BoundryCheck(positionOnGrid.x, positionOnGrid.y,
                    SelectedItem.Width, SelectedItem.Height));
                inventoryHighlight.SetSize(SelectedItem);
                inventoryHighlight.SetPosition(selectedInventoryGrid, SelectedItem, positionOnGrid.x, positionOnGrid.y);
            }
        }

        private Vector2Int GetTileGridPosition(Vector3 mousePosition)
        {
            if (SelectedItem != null)
            {
                mousePosition.x -= (SelectedItem.Width - 1) * InventoryGrid.TileSize / 2;
                mousePosition.y += (SelectedItem.Height - 1) * InventoryGrid.TileSize / 2;
            }

            return selectedInventoryGrid.GetTileGridPosition(mousePosition);
        }

        private void InsertItem(InventoryItem itemToInsert)
        {
            var positionOnGrid = selectedInventoryGrid.FindSpaceForObject(itemToInsert);
            if (positionOnGrid == null)
            {
                Destroy(itemToInsert);
                return;
            }

            selectedInventoryGrid.PlaceItem(itemToInsert, positionOnGrid.Value.x, positionOnGrid.Value.y);
        }

        private void CreateItem(InventoryItem item)
        {
            if (SelectedItem != null) return;
            var inventoryItem = Instantiate(item);
            SelectedItem = inventoryItem;
            rectTransform = inventoryItem.GetComponent<RectTransform>();
        }

        private void FastMovingItem(Vector2Int tileGridPosition, InventoryItem item = null)
        {
            if (item == null)
                PickUpItem(tileGridPosition, false);

            if (SelectedItem == null) return;
            if (SelectedInventoryGrid.Equals(LocationInventory.Instance.LocationInventoryGrid) ||
                SelectedInventoryGrid.Equals(CraftInventory.Instance.CraftInventoryGrid))
            {
                var complete = PutItemInCharactersInventory(SelectedItem);

                if (complete)
                {
                    AudioManager.Instance.PlaySFX(SelectedItem.OnPlaceAudioClip);
                    SelectedItem = null;
                    GroupInventoryChanged?.Invoke();
                }
                else
                {
                    if (SelectedInventoryGrid.Equals(CraftInventory.Instance.CraftInventoryGrid))
                    {
                        ThrowItemAtLocation(SelectedItem);
                        AudioManager.Instance.PlaySFX(SelectedItem.OnPlaceAudioClip);
                        SelectedItem = null;
                        GroupInventoryChanged?.Invoke();
                    }
                    else
                        PlaceItem(new Vector2Int(SelectedItem.OnGridPositionX,
                            SelectedItem.OnGridPositionY), false);
                }
            }
            else
            {
                ThrowItemAtLocation(SelectedItem);
                AudioManager.Instance.PlaySFX(SelectedItem.OnPlaceAudioClip);
                SelectedItem = null;
                GroupInventoryChanged?.Invoke();
            }
        }

        private bool PutItemInCharactersInventory(InventoryItem inventoryItem)
        {
            var complete = false;
            foreach (var character in TheRevenantsAge.GlobalMapController.ChosenGroup.CurrentGroupMembers)
            {
                var allInventoryStates = character.GetAllInventoryStates();
                if (!allInventoryStates.Any()) continue;
                foreach (var inventoryState in allInventoryStates)
                {
                    complete = inventoryState.InsertItem(inventoryItem);
                    if (complete) break;
                }

                if (complete) break;
            }

            return complete;
        }

        private void FastMovingItemsSameType(Vector2Int tileGridPosition)
        {
            PickUpItem(tileGridPosition, false);
            if (SelectedItem == null) return;

            List<BaseItem> allItemsSameType;
            if (SelectedInventoryGrid.Equals(LocationInventory.Instance.LocationInventoryGrid))
                allItemsSameType = LocationInventory.GetItemsFromInventoryByType<BaseItem>()
                    .Where(item => item.Data.Equals(SelectedItem.Data)).ToList();
            else if (SelectedInventoryGrid.Equals(CraftInventory.Instance.CraftInventoryGrid))
                allItemsSameType = CraftInventory.GetItemsFromInventoryByType<BaseItem>()
                    .Where(item => item.Data.Equals(SelectedItem.Data)).ToList();
            else
                allItemsSameType = SelectedInventoryGrid.GetItemsFromInventoryByType<BaseItem>()
                    .Where(item => item.Data.Equals(SelectedItem.Data)).ToList();

            FastMovingItem(tileGridPosition, SelectedItem);
            foreach (var item in allItemsSameType)
            {
                var inventoryItem = item.GetComponent<InventoryItem>();
                FastMovingItem(new Vector2Int(inventoryItem.OnGridPositionX, inventoryItem.OnGridPositionY));
            }
        }
    }
}