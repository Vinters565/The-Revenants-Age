using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using Inventory.SpecialCells;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace TheRevenantsAge
{
    public interface IGlobalMapCharacter : IGlobalMapEntity, ICharacter, ITurnEndAction
    {
        public int MaxOnGlobalMapEndurance { get; set; }
        public int CurrentOnGlobalMapEndurance { get; set; }

        public new Gun PrimaryGun { get; set; }
        public new Gun SecondaryGun { get; set; }
        public new MeleeWeapon MeleeWeapon { get; set; }

        public event Action<MeleeWeapon> MeleeWeaponChanged;
        public event Action<Gun, GunType> GunsChanged;

        public void Eat(EatableFood food);

        public IEnumerable<InventoryItem> Cook(CookableFood food)
        {
            //TODO Добавить опыт к навыку готовки
            Skills.Cooking.Develop(10);
            return food.Cook();
        }
        
        public void Heal(Medicine medicine, BodyPart bodyPartToHeal, bool shouldRemoveProperties, bool shouldHealHp)
        {
            //TODO добавить вляиние навыка медицины на лечение
            if (shouldHealHp)
                medicine.HealOnlyHp(bodyPartToHeal);
            if (medicine == null)
                return;
            if (shouldRemoveProperties)
                medicine.HealOnlyProperties(bodyPartToHeal);
        }

        public IEnumerable<InventoryItem> Loot(LocationData infoAboutLocation, int amountOfTimes)
        {
            //TODO Добавить опыт к навыку лутания в зависмости от редкости найденной вещи
            for (int i = 0; i < amountOfTimes; i++)
            {
                ManBody.Energy -= 1;
                var item = infoAboutLocation.GetLoot();
                if (item != null)
                {
                    Statistics.lootFound += 1;
                    Skills.Looting.Develop(5);
                }

                yield return item;
            }

            var rnd = new System.Random();

            if (rnd.NextDouble() <= Skills.Looting.ChanceToFindExtraItem)
            {
                yield return infoAboutLocation.GetLoot();
            }

            if (Skills.Looting.CanFreeLoot)
            {
                ManBody.Energy++;
            }
        }

        public IEnumerable<T> GetItemsFromAllInventoriesByType<T>()
            where T : MonoBehaviour
        {
            var result = new List<T>();
            //TODO: сделать более универсальным
            result.AddRange(ManBody.Chest.GetItemsFromInventory()
                .Where(x => x.GetComponent<T>())
                .Select(x => x.GetComponent<T>()));

            result.AddRange(ManBody.LeftLeg.GetItemsFromInventory()
                .Where(x => x.GetComponent<T>())
                .Select(x => x.GetComponent<T>()));


            return result;
        }

        public IEnumerable<InventoryState> GetAllInventoryStates()
        {
            var result = new List<InventoryState>();
            //TODO: сделать более универсальным
            result.AddRange(ManBody.Chest.GetAllInventoryStates());
            result.AddRange(ManBody.LeftLeg.GetAllInventoryStates());

            return result;
        }
    }
}