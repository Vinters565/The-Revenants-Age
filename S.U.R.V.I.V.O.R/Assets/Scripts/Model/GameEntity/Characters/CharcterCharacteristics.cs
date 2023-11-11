using System.Runtime.Serialization;

namespace TheRevenantsAge
{
    [DataContract]
    public class CharacterCharacteristics
    {
        //Уровни крафта
        [DataMember] private int currentHealingLevel;
        [DataMember] private int currentCraftLevel;
        [DataMember] private int currentCookingLevel;

        //Боевые характеристики
        [DataMember] private int meleeDamage;
        [DataMember] private int pointsOfAction;    
        [DataMember] private int pointsOfTurn;
        [DataMember] private int initiative;
        [DataMember] private float accuracy;
        [DataMember] private float recoilModifier;
        [DataMember] private int hidingNoice;
        
        //Глобальная карта
        [DataMember] private int maxWeightToGrab;
        [DataMember] private int onGlobalMapEndurance;
        [DataMember] private float immunityTics;
        [DataMember] private int coldResistance;
        [DataMember] private int maxPeopleInGroup;
        [DataMember] private float healingHpModifier;
        [DataMember] private float healImmunityTicksModifier;
        [DataMember] private float chanceToFindExtraItem;
        [DataMember] private float noiceWhileLooting;
        [DataMember] private bool canFreeLoot;
        [DataMember] private float chanceToHunt;
        [DataMember] private float chanceToCutFish;


        public CharacterCharacteristics(
            int currentHealingLevel = 1,
            int currentCraftLevel = 1,
            int currentCookingLevel = 1,
            int meleeDamage = 10,
            int pointsOfAction = 3,
            int pointsOfTurn = 1,
            int initiative = 10, 
            float accuracy = 0,
            float recoilModifier = 1,
            int hidingNoice = 0,
            int maxWeightToGrab = 25,
            int onGlobalMapEndurance = 10,
            float immunityTics = 10,
            int coldResistance = 15,
            int maxPeopleInGroup = 1,
            float healingHpModifier = 1,
            float healImmunityTicksModifier = 1,
            float chanceToFindExtraItem = 0,
            float noiceWhileLooting = 10,
            bool canFreeLoot = false,
            float chanceToHunt = 0.3f,
            float chanceToCutFish = 0.3f)
        {
            this.currentHealingLevel = currentHealingLevel;
            this.currentCraftLevel = currentCraftLevel;
            this.currentCookingLevel = currentCookingLevel;
            this.meleeDamage = meleeDamage;
            this.pointsOfAction = pointsOfAction;
            this.pointsOfTurn = pointsOfTurn;
            this.initiative = initiative;
            this.accuracy = accuracy;
            this.recoilModifier = recoilModifier;
            this.hidingNoice = hidingNoice;
            this.maxWeightToGrab = maxWeightToGrab;
            this.onGlobalMapEndurance = onGlobalMapEndurance;
            this.immunityTics = immunityTics;
            this.coldResistance = coldResistance;
            this.maxPeopleInGroup = maxPeopleInGroup;
            this.healingHpModifier = healingHpModifier;
            this.healImmunityTicksModifier = healImmunityTicksModifier;
            this.chanceToFindExtraItem = chanceToFindExtraItem;
            this.noiceWhileLooting = noiceWhileLooting;
            this.canFreeLoot = canFreeLoot;
            this.chanceToHunt = chanceToHunt;
            this.chanceToCutFish = chanceToCutFish;
        }

        public int CurrentHealingLevel
        {
            get => currentHealingLevel;
            set => currentHealingLevel = value;
        }

        public int CurrentCraftLevel
        {
            get => currentCraftLevel;
            set => currentCraftLevel = value;
        }

        public int CurrentCookingLevel
        {
            get => currentCookingLevel;
            set => currentCookingLevel = value;
        }

        public int MeleeDamage
        {
            get => meleeDamage;
            set => meleeDamage = value;
        }

        public int PointsOfAction
        {
            get => pointsOfAction;
            set => pointsOfAction = value;
        }

        public int PointsOfTurn
        {
            get => pointsOfTurn;
            set => pointsOfTurn = value;
        }

        public int Initiative
        {
            get => initiative;
            set => initiative = value;
        }

        public float Accuracy
        {
            get => accuracy;
            set => accuracy = value;
        }

        public float RecoilModifier
        {
            get => recoilModifier;
            set => recoilModifier = value;
        }

        public int HidingNoice
        {
            get => hidingNoice;
            set => hidingNoice = value;
        }

        public int MaxWeightToGrab
        {
            get => maxWeightToGrab;
            set => maxWeightToGrab = value;
        }

        public int OnGlobalMapEndurance
        {
            get => onGlobalMapEndurance;
            set => onGlobalMapEndurance = value;
        }

        public float ImmunityTics
        {
            get => immunityTics;
            set => immunityTics = value;
        }

        public int ColdResistance
        {
            get => coldResistance;
            set => coldResistance = value;
        }

        public int MaxPeopleInGroup
        {
            get => maxPeopleInGroup;
            set => maxPeopleInGroup = value;
        }

        public float HealHpModifier
        {
            get => healingHpModifier;
            set => healingHpModifier = value;
        }

        public float HealImmunityTicksModifier
        {
            get => healImmunityTicksModifier;
            set => healImmunityTicksModifier = value;
        }

        public float ChanceToFindExtraItem
        {
            get => chanceToFindExtraItem;
            set => chanceToFindExtraItem = value;
        }

        public float NoiceWhileLooting
        {
            get => noiceWhileLooting;
            set => noiceWhileLooting = value;
        }

        public bool CanFreeLoot
        {
            get => canFreeLoot;
            set => canFreeLoot = value;
        }

        public float ChanceToHunt
        {
            get => chanceToHunt;
            set => chanceToHunt = value;
        }

        public float ChanceToCutFish
        {
            get => chanceToCutFish;
            set => chanceToCutFish = value;
        }
    }
}