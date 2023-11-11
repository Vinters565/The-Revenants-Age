using System.Linq;
using Audio;
using TheRevenantsAge;
using UnityEngine;

namespace Inventory.SpecialCells
{
    public class SpecialGunModuleCell : SpecialCell
    {
        [SerializeField] public GunModuleType type;
        private Gun currentGun;

        public Gun CurrentGun
        {
            get => currentGun;
            set
            {
                currentGun = value;
            }
        }

        protected override void PlaceItem(InventoryItem item)
        {
            base.PlaceItem(item);
            InventoryController.SelectedItem = null;
            CurrentGun.AddGunModule(PlacedItem.GetComponent<GunModule>());
            
            AudioManager.Instance.PlayOneShotSFX(item.OnPlaceAudioClip);
        }

        protected override void GiveItem()
        {
            CurrentGun.RemoveGunModule(PlacedItem.GetComponent<GunModule>());
            base.GiveItem();
        }

        protected override bool CanInsertIntoSlot(InventoryItem item)
        {
            if (item == null) return false;
            var gunModule = item.GetComponent<GunModule>();
            if (gunModule is null) return false;
            return  PlacedItem == null && gunModule.Data.ModuleType == type && CurrentGun is not null && CurrentGun.Data.AvailableGunModules.Contains(gunModule.Data.ModuleType);
        }
    }
}
