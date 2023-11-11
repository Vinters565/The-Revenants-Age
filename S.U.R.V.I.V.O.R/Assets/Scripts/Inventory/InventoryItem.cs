using System;
using Audio;
using Interface;
using Interface.Items;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Inventory
{
    [RequireComponent(typeof(BaseItem))]
    [DisallowMultipleComponent]
    public class InventoryItem : MonoBehaviour, ITooltipPart, IBaseItemComponent
    {
        private BaseItem baseItem;
        private Image image;
        private Image backgroung;

        private const string ITEM_BACKGROUND_PATH = "Items/Background/ItemBackground";
        private const string ITEM_IMAGE_PATH = "Items/Background/ItemImage";
        public int OnGridPositionX { get; set; }
        public int OnGridPositionY { get; set; }

        public bool IsRotated { get; private set; }

        //public Character ItemOwner { get; set; }
        public Vector3 OnAwakeRectTransformSize { get; private set; }
        public Vector3 OnAwakeRectTransformScale { get; private set; }

        public Image Image => image;
        public Image Backgroung => backgroung;

        public InventoryGrid InventoryGrid => transform.GetComponentInParent<InventoryGrid>();
        public int Height => !IsRotated ? Data.Size.y : Data.Size.x;
        public int Width => !IsRotated ? Data.Size.x : Data.Size.y;
        public BaseItemData Data => baseItem.Data;
        public BaseItem BaseItem => baseItem;

        public event Action<InventoryItem> Destroed;

        [SerializeField] public InventoryItemSoundsType inventoryItemSoundsType;
        public AudioClip OnPickUpAudioClip { get; private set; }
        public AudioClip OnPlaceAudioClip { get; private set; }

        public void Awake()
        {
            if (!Game.Is2D) return;

            baseItem = GetComponent<BaseItem>();

            DrawItem();

            OnPlaceAudioClip = OnPlaceAudioClip
                ? OnPlaceAudioClip
                : Sounds.GetAudioOnPlaceItemByType(inventoryItemSoundsType);
            OnPickUpAudioClip = OnPickUpAudioClip
                ? OnPickUpAudioClip
                : Sounds.GetAudioOnPickUpItemByType(inventoryItemSoundsType);
        }

        public void Rotate()
        {
            IsRotated = !IsRotated;
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.rotation = Quaternion.Euler(0, 0, IsRotated ? 90 : 0);
        }

        public void OnDestroy()
        {
            Destroed?.Invoke(this);
            Destroed = null;
        }


        public ComponentState CreateState()
        {
            return new InventoryItemState
            {
                positionInInventory = new Vector2Int(OnGridPositionX, OnGridPositionY),
                isRotated = IsRotated,
            };
        }

        public void Restore(ComponentState state)
        {
            if (state is not InventoryItemState inventoryItemState) return;
            OnGridPositionX = inventoryItemState.positionInInventory.x;
            OnGridPositionY = inventoryItemState.positionInInventory.y;
            if (inventoryItemState.isRotated)
                Rotate();
        }

        public void DrawItem()
        {
            var imageBackground = transform.Find("ItemBackground").GetComponent<Image>();
            SetItemSize(imageBackground);
            imageBackground.color = ItemRarityDrawer.GetColorByRarity(baseItem.Data.ItemType);
            imageBackground.raycastTarget = false;
            backgroung = imageBackground;
            
            var img = transform.Find("ItemImage").GetComponent<Image>();
            img.raycastTarget = true;
            SetItemSize(img);
            img.sprite = baseItem.Data.Icon;
            image = img;

            var itemRectTransform = img.GetComponent<RectTransform>();
            OnAwakeRectTransformScale = itemRectTransform.localScale;
            OnAwakeRectTransformSize = itemRectTransform.sizeDelta;
            GetComponent<RectTransform>().sizeDelta = itemRectTransform.sizeDelta;
        }

        private void SetItemSize(Image img)
        {
            var rectTransform = img.GetComponent<RectTransform>();
            var scaleFactor = GlobalMapController.Instance.MainCanvas.scaleFactor;
            var size = (Vector2) Data.Size * InventoryGrid.TileSize * scaleFactor;
            rectTransform.sizeDelta = size;
        }

        public TooltipPartDrawer GetTooltipPart()
        {
            var drawer = TooltipPartDrawer.InitPart();
            drawer.AddMainText(Data.ItemName);
            drawer.AddPlainText(Data.Description);
            return drawer;
        }
    }
}