using System.Collections.Generic;
using Interface.BodyIndicatorFolder;
using Inventory;
using Inventory.SpecialCells;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.PlayerInfoLayer
{
    public class PlayerLayerLogic : MonoBehaviour
    {
        private IGlobalMapCharacter currentCharacter;

        public IGlobalMapCharacter CurrentCharacter
        {
            get => currentCharacter;
            set
            {
                if(gameObject.activeInHierarchy)
                    UnsubscribeCharacterEvents();
                gameObject.SetActive(value != null);
                currentCharacter = value;
                if (value == null) return;
                if(gameObject.activeInHierarchy)
                    SubscribeCharacterEvents();
                Init(currentCharacter);
            }
        }

        [SerializeField] private BodyIndicator bodyIndicator;
        [SerializeField] private PlayerCharacteristicsPanel playerCharacteristicsPanel;
        [SerializeField] private SpecialGunCell primaryGunSlot;
        [SerializeField] private SkillsInfoPanel skillsInfoPanel;

        [SerializeField] private SpecialGunCell secondaryGunSlot;
        [SerializeField] private SpecialMeleeWeaponCell meleeWeaponSet;


        [SerializeField] private Text nameTextBox;

        [SerializeField] private SpecialClothCell hatCell;
        [SerializeField] private SpecialClothCell underwearCell;
        [SerializeField] private SpecialClothCell backpackCell;
        [SerializeField] private SpecialClothCell vestCell;
        [SerializeField] private SpecialClothCell jacketCell;
        [SerializeField] private SpecialClothCell pantsCell;
        [SerializeField] private SpecialClothCell bootsCell;

        private void Init(IGlobalMapCharacter characterToInit)
        {
            currentCharacter = characterToInit;

            var allCells = new List<SpecialClothCell>
            {
                hatCell,
                underwearCell,
                backpackCell,
                vestCell,
                jacketCell,
                pantsCell,
                bootsCell
            };

            foreach (var cell in allCells)
            {
                if (cell != null)
                {
                    cell.CurrentCharacter = CurrentCharacter;
                }
            }

            if (bodyIndicator != null)
                bodyIndicator.Character = CurrentCharacter;
            if (playerCharacteristicsPanel != null)
                playerCharacteristicsPanel.Player = CurrentCharacter;
            if (primaryGunSlot != null)
                primaryGunSlot.CurrentCharacter = CurrentCharacter;
            if (secondaryGunSlot != null)
                secondaryGunSlot.CurrentCharacter = CurrentCharacter;
            if (meleeWeaponSet != null)
                meleeWeaponSet.CurrentCharacter = CurrentCharacter;
            if (nameTextBox != null)
                nameTextBox.text = $"{CurrentCharacter.FirstName} {CurrentCharacter.SurName}";
            if (skillsInfoPanel != null)
                skillsInfoPanel.Init(currentCharacter);
            PlaceAllItems();
        }

        private void SubscribeCharacterEvents()
        {
            if (CurrentCharacter == null) return;
            CurrentCharacter.ManBody.WearChanged += OnWearChanged;
            CurrentCharacter.MeleeWeaponChanged += OnMeleeWeaponChanged;
        }

        private void UnsubscribeCharacterEvents()
        {
            if (CurrentCharacter == null) return;
            CurrentCharacter.ManBody.WearChanged -= OnWearChanged;
            CurrentCharacter.MeleeWeaponChanged -= OnMeleeWeaponChanged;
        }

        private void OnMeleeWeaponChanged(MeleeWeapon weapon)
        {
            if (meleeWeaponSet == null) return;
            if (CurrentCharacter is not null && CurrentCharacter.MeleeWeapon)
            {
                meleeWeaponSet.CheckNewItem(CurrentCharacter.MeleeWeapon.GetComponent<InventoryItem>());
                return;
            }
            meleeWeaponSet.CheckNewItem(null);
        }
    
        private void OnWearChanged(ClothType type)
        {
            switch (type)
            {
                case ClothType.Jacket:
                    jacketCell.CheckNewItem(currentCharacter.ManBody.Chest.Jacket?.GetComponent<InventoryItem>());
                    break;
                case ClothType.Backpack:
                    backpackCell.CheckNewItem(currentCharacter.ManBody.Chest.Backpack?.GetComponent<InventoryItem>());
                    break;
                case ClothType.Pants:
                    pantsCell.CheckNewItem(currentCharacter.ManBody.LeftLeg.Pants?.GetComponent<InventoryItem>());
                    break;
                case ClothType.Vest:
                    vestCell.CheckNewItem(currentCharacter.ManBody.Chest.Vest?.GetComponent<InventoryItem>());
                    break;
                case ClothType.Underwear:
                    underwearCell.CheckNewItem(currentCharacter.ManBody.Chest.Underwear?.GetComponent<InventoryItem>());
                    break;
                case ClothType.Boots:
                    bootsCell.CheckNewItem(currentCharacter.ManBody.LeftLeg.Boots?.GetComponent<InventoryItem>());
                    break;
                case ClothType.Hat:
                    hatCell.CheckNewItem(currentCharacter.ManBody.Head.Hat?.GetComponent<InventoryItem>());
                    break;
            }
        }

        private void PlaceAllItems()
        {
            CheckCellAfterWindowOpen(
                CurrentCharacter.ManBody.Chest.Vest != null
                    ? CurrentCharacter.ManBody.Chest.Vest.GetComponent<InventoryItem>()
                    : null, vestCell);
            CheckCellAfterWindowOpen(
                CurrentCharacter.ManBody.Chest.Backpack != null
                    ? CurrentCharacter.ManBody.Chest.Backpack.GetComponent<InventoryItem>()
                    : null, backpackCell);
            CheckCellAfterWindowOpen(
                CurrentCharacter.ManBody.LeftLeg.Pants != null
                    ? CurrentCharacter.ManBody.LeftLeg.Pants.GetComponent<InventoryItem>()
                    : null, pantsCell);
            CheckCellAfterWindowOpen(
                CurrentCharacter.ManBody.Head.Hat != null
                    ? CurrentCharacter.ManBody.Head.Hat.GetComponent<InventoryItem>()
                    : null, hatCell);
            CheckCellAfterWindowOpen(
                CurrentCharacter.ManBody.Chest.Underwear != null
                    ? CurrentCharacter.ManBody.Chest.Underwear.GetComponent<InventoryItem>()
                    : null, underwearCell);
            CheckCellAfterWindowOpen(
                CurrentCharacter.ManBody.LeftLeg.Boots != null
                    ? CurrentCharacter.ManBody.LeftLeg.Boots.GetComponent<InventoryItem>()
                    : null, bootsCell);
            CheckCellAfterWindowOpen(
                CurrentCharacter.ManBody.Chest.Jacket != null
                    ? CurrentCharacter.ManBody.Chest.Jacket.GetComponent<InventoryItem>()
                    : null, jacketCell);
            CheckCellAfterWindowOpen(
                CurrentCharacter.PrimaryGun != null ? CurrentCharacter.PrimaryGun.GetComponent<InventoryItem>() : null,
                primaryGunSlot);
            CheckCellAfterWindowOpen(
                CurrentCharacter.SecondaryGun != null ? CurrentCharacter.SecondaryGun.GetComponent<InventoryItem>() : null,
                secondaryGunSlot);
            CheckCellAfterWindowOpen(
                CurrentCharacter.MeleeWeapon != null ? CurrentCharacter.MeleeWeapon.GetComponent<InventoryItem>() : null,
                meleeWeaponSet);
        }

        private void CheckCellAfterWindowOpen(InventoryItem item, SpecialCell cell)
        {
            if (cell != null)
                cell.UpdateItem(item);
        }

        public void OnEnable()
        {
            if (currentCharacter == null) return;
            SubscribeCharacterEvents();
            PlaceAllItems();
            bodyIndicator.Character = currentCharacter;
        }
        
        public void OnDisable()
        {
            UnsubscribeCharacterEvents();
        }
    }
}