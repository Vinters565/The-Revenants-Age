using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public class BodyWearableClothes: Body, IWearClothes
    {
        protected IWearClothes[] wearClothesBodyParts;
        
        public event Action<ClothType> WearChanged;
        
        /// <summary>
        /// Подписка на аналогичные ивенты у BodyPathWearableClothes 
        /// </summary>
        public event Action<float, Vector3> WhenDamagedOrRecoveryArmor;
        protected override void Awake()
        {
            base.Awake();
            wearClothesBodyParts = BodyParts.OfType<IWearClothes>().ToArray();
            foreach (var bodyPart in wearClothesBodyParts)
            {
                bodyPart.WhenDamagedOrRecoveryArmor += BodyPartOnWhenDamagedOrRecoveryArmor;
            }
        }
        public bool Wear(Clothes clothesToWear)
        {
            if (clothesToWear == null)
                return false;
            var isSuccess = false;
            foreach (var wearClothesBodyPart in wearClothesBodyParts)
            {
                if (wearClothesBodyPart.Wear(clothesToWear))
                    isSuccess = true;
            }

            if (isSuccess) WearChanged?.Invoke(clothesToWear.Data.ClothType);
            return isSuccess;
        }


        public Clothes UnWear(ClothType clothType, bool isNeedToDropClothes = true)
        {
            Clothes clothes = null;
            foreach (var bodyPart in wearClothesBodyParts)
            {
                var x = bodyPart.UnWear(clothType);
                if (x is not null)
                    clothes = x;
            }
            if (clothes is not null)
            {
                if (isNeedToDropClothes)
                {
                    if(clothes.Inventory is not null)
                        foreach (var item in clothes.Inventory.GetItems())
                        {
                            clothes.Inventory.PickUpItem(item.OnGridPositionX, item.OnGridPositionY);
                            PlaceItemToInventory(item);//TODO Вывести игроку на экран, что вещи выпали
                        }
                }
                WearChanged?.Invoke(clothType);
            }
            return clothes;
        }

        public IEnumerable<Clothes> GetClothes()
        {
            var clothes = new List<Clothes>();
            foreach (var bodyPart in wearClothesBodyParts)
            {
                clothes.AddRange(bodyPart.GetClothes());
            }

            return clothes.Distinct().Where(x => x is not null);
        }
        
        public bool PlaceItemToInventory(InventoryItem itemToPlace)
        {
            var clothes = GetClothes();
            foreach (var cloth in clothes)
            {
                if (cloth.Inventory.InsertItem(itemToPlace))
                    return true;
            }

            if (LocationInventory.Instance.LocationInventoryGrid.InsertItem(itemToPlace))
                return true;
            Destroy(itemToPlace);
            return false;
        }
        
        private void BodyPartOnWhenDamagedOrRecoveryArmor(float diff, Vector3 pos)
        {
            WhenDamagedOrRecoveryArmor?.Invoke(diff, pos);
        }
    }
}