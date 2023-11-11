using System;
using System.Linq;
using UnityEngine;

namespace TheRevenantsAge
{
    public interface ICharacter: IEntityWithWeapon
    {
        public ManBody ManBody => (ManBody)Body;
        public string FirstName { get; }
        public string SurName { get; }
        public Sprite Sprite { get; }
        public CharacterSkills Skills { get; }
        public CharacterCharacteristics Characteristics { get; }
        public CharacterStatistics Statistics { get; }
        public Gun PrimaryGun { get; }
        public Gun SecondaryGun { get; }
        public MeleeWeapon MeleeWeapon {get;}
        public ChosenWeaponTypes ChosenWeaponType { get; }
        public event Action<Weapon, Weapon> ChosenWeaponChanged;
        
        public void SetChosenWeapon(ChosenWeaponTypes type);        
        public CharacterState CreateState();
        public void Restore(CharacterState state);
        
        public void Heal(Medicine medicine, ICharacter characterToHeal, bool shouldRemoveProperties, bool shouldHealHp)
        {
            //TODO добавить вляиние навыка медицины на лечение и добавление опыта к скиллу хила
            var orderedBp =
                characterToHeal.ManBody.BodyParts
                    .OrderBy(x => x.MaxHp - x.Hp)
                    .ToList();

            foreach (var bodyPart in orderedBp)
            {
                if (shouldRemoveProperties)
                    Statistics.healedHp += medicine.HealOnlyProperties(bodyPart);
                if (medicine == null)
                    return;
            }

            foreach (var bodyPart in orderedBp)
            {
                if (shouldHealHp)
                    Statistics.healedHp += medicine.HealOnlyHp(bodyPart);
                if (medicine == null)
                    return;
            }
        }
    }
}