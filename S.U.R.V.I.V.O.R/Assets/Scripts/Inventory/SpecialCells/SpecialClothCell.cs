using Audio;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Inventory.SpecialCells
{
    public class SpecialClothCell : SpecialCell
    {
        [SerializeField] private ClothType type;
        [SerializeField] private InventoryGrid currentInventory;
        private IGlobalMapCharacter currentCharacter;

        public IGlobalMapCharacter CurrentCharacter
        {
            get => currentCharacter;
            set
            {
                currentCharacter = value;
                Init();
            }
        }

        private bool wasOpened;

        public override void Init()
        {
            base.Init();
            if (currentInventory != null)
                currentInventory.InventoryOwner = CurrentCharacter;
            canvasTransform = TheRevenantsAge.GlobalMapController.Instance.MainCanvas.transform;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            if (currentInventory != null && PlacedItem == null)
                currentInventory.ChangeState(new InventoryState(Vector2Int.zero));
        }

        protected override void PlaceItem(InventoryItem item)
        {
            base.PlaceItem(item);
            bool isWeared = CurrentCharacter.ManBody.Wear(item.GetComponent<Clothes>());
            if (isWeared)
            {
                item.BaseItem.ItemOwner = CurrentCharacter;
                InventoryController.SelectedItem = null;
                AudioManager.Instance.PlayOneShotSFX(item.OnPlaceAudioClip);
            }
            else
            {
                GiveItem();
            }
        }

        protected override void GiveItem()
        {
            if(PlacedItem == null) return;
            var removedClothes = CurrentCharacter.ManBody.UnWear(PlacedItem.GetComponent<Clothes>().Data.ClothType);
            if (removedClothes is null) return;
            base.GiveItem();
            currentInventory?.ChangeState(new InventoryState(Vector2Int.zero));
        }

        protected override void ReDraw()
        {
            base.ReDraw();
            if (currentInventory != null)
                UpdateInventory();
        }

        private void UpdateInventory()
        {
            var item = CurrentCharacter.ManBody.GetClothByType(type);
            if (item != null)
                currentInventory.ChangeState(item.Inventory);
            else
            {
                currentInventory.ChangeState(new InventoryState(Vector2Int.zero));
            }
        }

        protected override bool CanInsertIntoSlot(InventoryItem item)
        {
            return PlacedItem == null && item && item.GetComponent<Clothes>() &&
                   item.GetComponent<Clothes>().Data.ClothType == type;
        }
    }
}