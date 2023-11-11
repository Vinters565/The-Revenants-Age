using System;
using TheRevenantsAge;
using UnityEngine;

namespace Interface.PlayerInfoLayer
{
    public class SkillsInfoPanel : MonoBehaviour //Никакие навыки невозможно прокачать при открытой панели персонажа, поэтому привязки к ивентам на скилл нет.
    {
        [Header("Guns")]
        [SerializeField] private SkillInfo pistolHandling;
        [SerializeField] private SkillInfo autoPistolHandling;
        [SerializeField] private SkillInfo throwableHandling;
        [SerializeField] private SkillInfo assaultRifleHandling;
        [SerializeField] private SkillInfo snipeRifleHandling;
        [SerializeField] private SkillInfo semiAutomaticRifleHandling;
        [SerializeField] private SkillInfo shotgunHandling;
        [SerializeField] private SkillInfo shortMeleeHandling;

        [SerializeField] private SkillInfo longMeleeHandling;
        //

        [Header("Body")]
        [SerializeField] private SkillInfo durabilityInfo;
        [SerializeField] private SkillInfo strengthInfo;
        [SerializeField] private SkillInfo survivalInfo;

        [SerializeField] private SkillInfo warmbrodedInfo;
        //

        [Header("Skills")]
        [SerializeField] private SkillInfo accuracyInfo;
        [SerializeField] private SkillInfo craftInfo;
        [SerializeField] private SkillInfo lootingInfo;
        [SerializeField] private SkillInfo stealthInfo;
        [SerializeField] private SkillInfo cookingInfo;
        [SerializeField] private SkillInfo charismaInfo;
        [SerializeField] private SkillInfo fishingInfo;
        [SerializeField] private SkillInfo huntingInfo;
        [SerializeField] private SkillInfo firstAidInfo;

        private IGlobalMapCharacter currentCharacter;

        public void Init(IGlobalMapCharacter character)
        {
            //Оружие
            pistolHandling.Init(character.Skills.GetGunHandlingSkillLevel(HandlingTypes.Pistol));
            autoPistolHandling.Init(character.Skills.GetGunHandlingSkillLevel(HandlingTypes.AutoPistol));
            throwableHandling.Init(character.Skills.GetGunHandlingSkillLevel(HandlingTypes.Throwable));
            assaultRifleHandling.Init(character.Skills.GetGunHandlingSkillLevel(HandlingTypes.AssaultRifle));
            snipeRifleHandling.Init(character.Skills.GetGunHandlingSkillLevel(HandlingTypes.SnipeRifle));
            semiAutomaticRifleHandling.Init(character.Skills.GetGunHandlingSkillLevel(HandlingTypes.SemiAutoRifle));
            shotgunHandling.Init(character.Skills.GetGunHandlingSkillLevel(HandlingTypes.Shotgun));
            shortMeleeHandling.Init(character.Skills.GetGunHandlingSkillLevel(HandlingTypes.ShortMelee));
            longMeleeHandling.Init(character.Skills.GetGunHandlingSkillLevel(HandlingTypes.LongMelee));
            //

            // Тело
            durabilityInfo.Init(character.Skills.Durability.CurrentLevel + 1);
            strengthInfo.Init(character.Skills.Strength.CurrentLevel + 1);
            survivalInfo.Init(character.Skills.Survival.CurrentLevel + 1);
            warmbrodedInfo.Init(character.Skills.Warmbrodeed.CurrentLevel + 1);
            //
            accuracyInfo.Init(character.Skills.Accuracy.CurrentLevel + 1);
            craftInfo.Init(character.Skills.Crafting.CurrentLevel + 1);
            lootingInfo.Init(character.Skills.Looting.CurrentLevel + 1);
            stealthInfo.Init(character.Skills.Stealth.CurrentLevel + 1);
            cookingInfo.Init(character.Skills.Cooking.CurrentLevel + 1);
            charismaInfo.Init(character.Skills.Charisma.CurrentLevel + 1);
            fishingInfo.Init(character.Skills.Fishing.CurrentLevel + 1);
            huntingInfo.Init(character.Skills.Hunting.CurrentLevel + 1);
            firstAidInfo.Init(character.Skills.Healing.CurrentLevel + 1);

            currentCharacter = character;
        }

        public void OnEnable()
        {
            if (currentCharacter == null) return;
            var skills = currentCharacter.Skills;
            pistolHandling.SetValue(skills.GetGunHandlingSkill(HandlingTypes.Pistol));
            autoPistolHandling.SetValue(skills.GetGunHandlingSkill(HandlingTypes.AutoPistol));
            throwableHandling.SetValue(skills.GetGunHandlingSkill(HandlingTypes.Throwable));
            assaultRifleHandling.SetValue(skills.GetGunHandlingSkill(HandlingTypes.AssaultRifle));
            snipeRifleHandling.SetValue(skills.GetGunHandlingSkill(HandlingTypes.SnipeRifle));
            semiAutomaticRifleHandling.SetValue(skills.GetGunHandlingSkill(HandlingTypes.SemiAutoRifle));
            shotgunHandling.SetValue(skills.GetGunHandlingSkill(HandlingTypes.Shotgun));
            shortMeleeHandling.SetValue(skills.GetGunHandlingSkill(HandlingTypes.ShortMelee));
            longMeleeHandling.SetValue(skills.GetGunHandlingSkill(HandlingTypes.LongMelee));

            durabilityInfo.SetValue(skills.Durability);
            strengthInfo.SetValue(skills.Strength);
            survivalInfo.SetValue(skills.Survival);
            warmbrodedInfo.SetValue(skills.Warmbrodeed);

            accuracyInfo.SetValue(skills.Accuracy);
            craftInfo.SetValue(skills.Crafting);
            lootingInfo.SetValue(skills.Looting);
            stealthInfo.SetValue(skills.Stealth);
            cookingInfo.SetValue(skills.Cooking);
            charismaInfo.SetValue(skills.Charisma);
            fishingInfo.SetValue(skills.Fishing);
            huntingInfo.SetValue(skills.Hunting);
            firstAidInfo.SetValue(skills.Healing);
        }
    }
}