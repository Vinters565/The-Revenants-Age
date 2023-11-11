using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Extension;
using Inventory;
using Inventory.SpecialCells;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public abstract class BodyPathWearableClothes : BodyPart, IWearClothes
    {
        [NamedArray][SerializeField] private List<ClothType> possibleClothTypes;
        protected Dictionary<ClothType, Clothes> clothesDict;
        public float CurrentArmor => (clothesDict != null)?clothesDict.Values.Where(x => x != null).Sum(x => x.CurrentArmor) : 0;
        public bool IsArmorDestroyed => CurrentArmor == 0f;
        
        /// <summary>
        /// Вызывается при нанесении урона по броне или ее востановлении
        /// Первый аргумент - это изменение, если меньше, то это урон, если больше, то восстановление
        /// Второй аргумент - это местонахождение
        /// </summary>
        public event Action<float, Vector3> WhenDamagedOrRecoveryArmor;
        
        protected override void Awake()
        {
            base.Awake();
            clothesDict = new Dictionary<ClothType, Clothes>();
            foreach (var possibleClothType in possibleClothTypes)
                clothesDict.Add(possibleClothType, null);
        }
        
        public bool Wear(Clothes clothesToWear)
        {
            if (clothesToWear == null || !clothesDict.ContainsKey(clothesToWear.Data.ClothType) ||
                clothesDict[clothesToWear.Data.ClothType] != null)
                return false;

            clothesDict[clothesToWear.Data.ClothType] = clothesToWear;
            clothesToWear.ArmorChanged += ClothesToWearOnArmorChanged;
            return true;
        }



        public Clothes UnWear(ClothType clothType, bool isNeededToDropItem)
        {
            try
            {
                var removedClothes = clothesDict[clothType];
                clothesDict[clothType] = null;
                removedClothes.ArmorChanged -= ClothesToWearOnArmorChanged;
                return removedClothes;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<Clothes> GetClothes() => clothesDict.Values;

        public IEnumerable<InventoryItem> GetItemsFromInventory()
        {
            if (clothesDict == null || clothesDict.Values == null || clothesDict.Values.Count == 0) return null;
            return clothesDict.Values
                .Where(x => x != null && x.Inventory != null)
                .SelectMany(x => x.Inventory.GetItems());
        } 
        
        public IEnumerable<InventoryState> GetAllInventoryStates()
        {
            if (clothesDict == null || clothesDict.Values == null || clothesDict.Values.Count == 0) return null;
            return clothesDict.Values
                .Where(x => x != null && x.Inventory != null)
                .Select(x => x.Inventory);
        }

        public void DamageArmor(float damage)
        {
            if (damage < 0) throw new ArgumentException("Урон по броне меньше нуля");
            var cloth = clothesDict
                .Values
                .Where(x => x!= null)
                .OrderBy(x => x.CurrentArmor)
                .FirstOrDefault();
            if (cloth is null)
                return;
            cloth.CurrentArmor = damage <= cloth.CurrentArmor ? cloth.CurrentArmor - damage : 0;
        }
        
        private void ClothesToWearOnArmorChanged(float before, float after)
        {
            WhenDamagedOrRecoveryArmor?.Invoke(after- before, GetMyPosition());
        }
    }
}