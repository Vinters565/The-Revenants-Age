using System.Collections.Generic;
using Audio;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Inventory.SpecialCells
{
    public class SpecialMeleeWeaponCell : SpecialCell
    {
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

        protected override void PlaceItem(InventoryItem item)
        {
            base.PlaceItem(item);
            if (item.Data.Size.y > item.Data.Size.x && !item.IsRotated)
            {
                item.Rotate();
            }
            item.BaseItem.ItemOwner = CurrentCharacter;
            CurrentCharacter.MeleeWeapon = item.GetComponent<MeleeWeapon>();
            InventoryController.SelectedItem = null;
            
            AudioManager.Instance.PlayOneShotSFX(item.OnPlaceAudioClip);
        }

        protected override void GiveItem()
        {
            base.GiveItem();
            currentCharacter.MeleeWeapon = null;
        }

        protected override bool CanInsertIntoSlot(InventoryItem item)
        {
            return PlacedItem == null && item && item.GetComponent<MeleeWeapon>();
        }
    }
}