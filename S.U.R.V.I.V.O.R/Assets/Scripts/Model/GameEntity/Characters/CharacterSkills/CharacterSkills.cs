using System.Collections.Generic;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public class CharacterSkills
    {
        //hp
        // Body
        public Strength Strength { get; }
        public Durability Durability { get; }
        public Survival Survival { get; }
        public Warmbrodeed Warmbrodeed { get; }
        //

        //Skills
        public Charisma Charisma { get; }

        public Healing Healing { get; }

        public Crafting Crafting { get; }

        public Cooking Cooking { get; }

        public Looting Looting { get; }

        public Accuracy Accuracy { get; }

        public Hunting Hunting { get; }

        public Fishing Fishing { get; }

        public Stealth Stealth { get; }
        //

        private Dictionary<HandlingTypes, GunHandlingSkill> gunHandlingSkills;

        public int GetGunHandlingSkillLevel(HandlingTypes type)
        {
            return gunHandlingSkills[type].CurrentLevel + 1;
        }

        public GunHandlingLevel GetSkillsInfluenceOnShot(Gun gun)
        {
            return gunHandlingSkills[gun.HandlingType].CurrentHandlingLevel;
        }
        
        public GunHandlingSkill GetGunHandlingSkill(HandlingTypes type) => gunHandlingSkills[type];

        public void AddExperienceToShootingSkill(Gun gun, float exp)
        {
            gunHandlingSkills[gun.HandlingType].Develop(exp);
        }

        public CharacterSkills(ICharacter character)
        {
            Strength = new Strength(character, 5);
            Durability = new Durability(character, 3);
            Survival = new Survival(character, 2);
            Warmbrodeed = new Warmbrodeed(character, 3);
            Charisma = new Charisma(character, 3);

            Healing = new Healing(character,3);
            Crafting = new Crafting(character,3);
            Cooking = new Cooking(character,3);
            Looting = new Looting(character);
            Accuracy = new Accuracy(character);
            Hunting = new Hunting(character);
            Fishing = new Fishing(character);
            Stealth = new Stealth(character,3);


            gunHandlingSkills = new Dictionary<HandlingTypes, GunHandlingSkill>
            {
                { HandlingTypes.Pistol, new PistolHandling() },
                { HandlingTypes.AutoPistol, new AutoPistolHandling(2) },
                { HandlingTypes.Throwable, new ThrowableHandling() },
                { HandlingTypes.AssaultRifle, new AssaultRifleHandling(7) },
                { HandlingTypes.SnipeRifle, new SnipeRifleHandling() },
                { HandlingTypes.SemiAutoRifle, new SemiAutoRifleHandling(2) },
                { HandlingTypes.Shotgun, new ShotgunHandling(3) },
                { HandlingTypes.ShortMelee, new ShortMeleeHandling(2) },
                { HandlingTypes.LongMelee, new LongMeleeHandling() }
            };
        }


        public SkillsData CreateState()
        {
            return new SkillsData()
            {
                strength = Strength.CreateState(),
                durability = Durability.CreateState(),
                survival = Survival.CreateState(),
                warmbrodeed = Warmbrodeed.CreateState(),

                charisma = Charisma.CreateState(),
                healing = Healing.CreateState(),
                crafting = Crafting.CreateState(),
                cooking = Cooking.CreateState(),
                looting = Looting.CreateState(),
                accuracy = Accuracy.CreateState(),
                hunting = Hunting.CreateState(),
                fishing = Fishing.CreateState(),
                stealth = Stealth.CreateState(),
                
                pistolHandlingSkill = gunHandlingSkills[HandlingTypes.Pistol].CreateState(),
                autoPistolHandlingSkill = gunHandlingSkills[HandlingTypes.AutoPistol].CreateState(),
                throwableHandlingSkill = gunHandlingSkills[HandlingTypes.Throwable].CreateState(),
                assaultRifleHandlingSkill = gunHandlingSkills[HandlingTypes.AssaultRifle].CreateState(),
                snipeRifleHandlingSkill = gunHandlingSkills[HandlingTypes.SnipeRifle].CreateState(),
                semiAutoRifleHandlingSkill = gunHandlingSkills[HandlingTypes.SemiAutoRifle].CreateState(),
                shotgunHandlingSkill = gunHandlingSkills[HandlingTypes.Shotgun].CreateState(),
                shortMeleeHandlingSkill = gunHandlingSkills[HandlingTypes.ShortMelee].CreateState(),
                longMeleeHandlingSkill = gunHandlingSkills[HandlingTypes.LongMelee].CreateState()
            };
        }

        public void Restore(SkillsData state)
        {
            Strength.Restore(state.strength);
            Durability.Restore(state.durability);
            Survival.Restore(state.survival);
            Warmbrodeed.Restore(state.warmbrodeed);
            
            
            Charisma.Restore(state.charisma);
            Healing.Restore(state.healing);
            Crafting.Restore(state.crafting);
            Cooking.Restore(state.cooking);
            Looting.Restore(state.looting);
            Accuracy.Restore(state.accuracy);
            Hunting.Restore(state.hunting);
            Fishing.Restore(state.fishing);
            Stealth.Restore(state.stealth);
            

            gunHandlingSkills[HandlingTypes.Pistol].Restore(state.pistolHandlingSkill);
            gunHandlingSkills[HandlingTypes.AutoPistol].Restore(state.autoPistolHandlingSkill);
            gunHandlingSkills[HandlingTypes.Throwable].Restore(state.throwableHandlingSkill);
            gunHandlingSkills[HandlingTypes.AssaultRifle].Restore(state.assaultRifleHandlingSkill);
            gunHandlingSkills[HandlingTypes.SnipeRifle].Restore(state.snipeRifleHandlingSkill);
            gunHandlingSkills[HandlingTypes.SemiAutoRifle].Restore(state.semiAutoRifleHandlingSkill);
            gunHandlingSkills[HandlingTypes.Shotgun].Restore(state.shotgunHandlingSkill);
            gunHandlingSkills[HandlingTypes.ShortMelee].Restore(state.shortMeleeHandlingSkill);
            gunHandlingSkills[HandlingTypes.LongMelee].Restore(state.longMeleeHandlingSkill);
        }
    }
}