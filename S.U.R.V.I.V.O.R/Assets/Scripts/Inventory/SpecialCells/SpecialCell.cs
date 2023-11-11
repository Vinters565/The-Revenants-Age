using Audio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.SpecialCells
{
    public enum GunType
    {
        PrimaryGun,
        SecondaryGun,
        MeleeWeapon
    }
    
    public enum GunModuleType
    {
        Grip,
        Spring,
        Shutter,
        Scope,
        Suppressor,
        Tactical,
        Magazine
    }

    public abstract class SpecialCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject SpecialCellHighliter;
        [SerializeField] protected Transform canvasTransform;
        protected InventoryController InventoryController { get; private set; }
        protected InventoryItem placedItem;
        private bool isPointerOverCell;
        private Image image;

        public bool IsPointerOverCell
        {
            get => isPointerOverCell;
            set
            {
                isPointerOverCell = value;
                if (InventoryController != null)
                    InventoryController.isPointerUnderInventory = value;
            }
        }

        public virtual InventoryItem PlacedItem
        {
            get => placedItem;

            protected set
            {
                placedItem = value;
                image.color = image.color.WithAlpha(value ? 0 : 255);
            }
        }

        public virtual void Init()
        {
            if (InventoryController == null)
                InventoryController = InventoryController.Instance;
            InventoryController.SelectedItemChanged += OnSelectedItemChanged;
            image = GetComponent<Image>();
        }

        protected abstract bool CanInsertIntoSlot(InventoryItem item);

        protected virtual void ReDraw()
        {
            DrawItem();
        }

        protected void DrawItem()
        {
            if (PlacedItem == null || PlacedItem.gameObject == null) return;
            var rectTransform = PlacedItem.gameObject.GetComponent<RectTransform>();
            PlacedItem.gameObject.SetActive(true);
            rectTransform.SetParent(GetComponent<RectTransform>());
            rectTransform.anchoredPosition = new Vector2(0,0);
            PlacedItem.GetComponent<RectTransform>().sizeDelta = PlacedItem.OnAwakeRectTransformSize;
            PlacedItem.GetComponent<RectTransform>().localScale = PlacedItem.OnAwakeRectTransformScale;
            ChangeItemSize(PlacedItem,GetComponent<RectTransform>());
        }
    
        public void Update()
        {
            if (Input.GetMouseButtonDown(0) && IsPointerOverCell)
            {
                if (PlacedItem == null && InventoryController.SelectedItem != null)
                {
                    if (CanInsertIntoSlot(InventoryController.SelectedItem))
                    {
                        PlaceItem(InventoryController.SelectedItem);
                    }
                }
                else if (InventoryController.SelectedItem == null)
                {
                    GiveItem();
                }
            }
        }

        protected virtual void PlaceItem(InventoryItem item)
        {
            if(item.IsRotated)
                item.Rotate();
            PlacedItem = item;
        }
    
        public virtual void UpdateItem(InventoryItem item)
        {
            if (PlacedItem == null)
            {
                if (item != null)
                {
                    item.gameObject.SetActive(true);
                    PlacedItem = item;
                }
            }
            else
            {
                if (item != PlacedItem && PlacedItem.transform.IsChildOf(transform))
                {
                    PlacedItem.gameObject.SetActive(false);
                    PlacedItem = item;
                }
            }
            ReDraw();
        }


        protected virtual void PlaceNullItem()
        {
            PlacedItem = null;
            ReDraw();
        }

        protected virtual void GiveItem()
        {
            if (PlacedItem == null) return;

            if (PlacedItem.IsRotated)
                PlacedItem.Rotate();
            
            var imgRect = PlacedItem.GetComponent<RectTransform>();
            var bgRect = PlacedItem.Backgroung.GetComponent<RectTransform>();
            
            imgRect.sizeDelta = PlacedItem.OnAwakeRectTransformSize;
            imgRect.localScale = PlacedItem.OnAwakeRectTransformScale;
            
            bgRect.sizeDelta = PlacedItem.OnAwakeRectTransformSize;
            bgRect.localScale = PlacedItem.OnAwakeRectTransformScale;
            
            PlacedItem.GetComponent<RectTransform>().SetParent(canvasTransform);
            InventoryController.PickUpItemFromSpecialCell(PlacedItem);
            AudioManager.Instance.PlayOneShotSFX(PlacedItem.OnPickUpAudioClip);
            PlaceNullItem();
        }

        private void ChangeItemSize(InventoryItem inventoryItem, RectTransform cellTransform)
        {
            var itemTransform = inventoryItem.GetComponent<RectTransform>();
            var bgTransform = inventoryItem.Backgroung.GetComponent<RectTransform>();
            
            var width = inventoryItem.IsRotated ? cellTransform.rect.height: cellTransform.rect.width;
            var height = inventoryItem.IsRotated ? cellTransform.rect.width: cellTransform.rect.height;
            var bgSize = inventoryItem.IsRotated
                ? new Vector2(cellTransform.sizeDelta.y, cellTransform.sizeDelta.x)
                : new Vector2(cellTransform.sizeDelta.x, cellTransform.sizeDelta.y);
            
            ChangeItemWidthHeight(itemTransform, width, height);
            bgTransform.sizeDelta = bgSize / itemTransform.localScale;
        }

        public virtual void CheckNewItem(InventoryItem item)
        {
            if (item == null || item == PlacedItem)
            {
                ReDraw();
            }
            else if (item != PlacedItem && PlacedItem != null)
            {
                PlacedItem.gameObject.SetActive(false);
                PlacedItem = item;
                item.gameObject.SetActive(true);
                ReDraw();
            }
            else if (item != PlacedItem && PlacedItem == null)
            {
                PlacedItem = item;
                PlacedItem.gameObject.SetActive(true);
                ReDraw();
            }
        }
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsPointerOverCell = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsPointerOverCell = false;
        }

        protected virtual void OnEnable()
        {
            InventoryController.Instance.SelectedItemChanged += OnSelectedItemChanged;
        }
        
        protected virtual void OnDisable()
        {
            InventoryController.Instance.SelectedItemChanged -= OnSelectedItemChanged;
        }
        
        protected virtual void OnDestroy()
        {
            InventoryController.Instance.SelectedItemChanged -= OnSelectedItemChanged;
        }

        protected void OnSelectedItemChanged(InventoryItem changedItem)
        {
            if (SpecialCellHighliter == null) return;
            if (changedItem == null || !CanInsertIntoSlot(changedItem))
            {
                SpecialCellHighliter.SetActive(false);
            }
            if (CanInsertIntoSlot(changedItem))
            {
                SpecialCellHighliter.SetActive(true);
            }
        }
        
        public static void ChangeItemWidthHeight(RectTransform itemTransform, float cellWidth, float cellHeight)
        {
            if (itemTransform.rect.width > cellWidth || itemTransform.rect.height > cellHeight)
            {
                if (itemTransform.rect.width > cellWidth && itemTransform.rect.height > cellHeight)
                {
                    var yToScale = 1f;
                    var xToScale = 1f;
                    if (itemTransform.rect.width < itemTransform.rect.height)
                    {
                        yToScale = cellHeight / itemTransform.rect.height;
                        if (itemTransform.rect.width * yToScale > cellWidth)
                        {
                            xToScale = cellWidth / (itemTransform.rect.width * yToScale);
                        }
                    }
                    else
                    {
                        xToScale = cellWidth / itemTransform.rect.width;
                        if (itemTransform.rect.height * xToScale > cellHeight)
                        {
                            yToScale = cellHeight / (itemTransform.rect.height * xToScale);
                        }
                    }

                    itemTransform.localScale *= xToScale * yToScale;
                }
                else if (itemTransform.rect.width > cellWidth && itemTransform.rect.width >= itemTransform.rect.height)
                {
                    itemTransform.localScale *= cellWidth / itemTransform.rect.width;
                }
                else if (itemTransform.rect.height > cellHeight && itemTransform.rect.height >= itemTransform.rect.width)
                {
                    itemTransform.localScale *= cellHeight / itemTransform.rect.height;
                }
            }
        }
    }
}