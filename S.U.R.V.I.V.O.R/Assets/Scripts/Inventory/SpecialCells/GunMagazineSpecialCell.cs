using Audio;
using TheRevenantsAge;
using UnityEngine;

namespace Inventory.SpecialCells
{
    public class GunMagazineSpecialCell : SpecialCell
    {
        public SpecialGunCell gunCell;
        protected override void PlaceItem(InventoryItem item)
        {
            base.PlaceItem(item);
            var x = gunCell.GetGun();
            InventoryController.SelectedItem =
                x.Reload(PlacedItem.GetComponent<Magazine>())?.GetComponent<InventoryItem>();

            AudioManager.Instance.PlayOneShotSFX(item.OnPlaceAudioClip);
        }

        protected override void GiveItem()
        {
            base.GiveItem();
            gunCell.PlacedItem.GetComponent<Gun>().Reload(null);
        }

        protected override bool CanInsertIntoSlot(InventoryItem item)
        {
            if (item == null || PlacedItem != null) return false;
            var magazine = item.GetComponent<Magazine>();
            return magazine && gunCell.PlacedItem &&
                   magazine.Data.Caliber == gunCell.PlacedItem.GetComponent<Gun>().Data.Caliber;
        }
    }
}