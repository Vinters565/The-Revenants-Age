using System.Collections.Generic;
using Interface.PlayerInfoLayer;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public class Healing: ActiveSkill<HealingLevel>
    {
        public float CurrentHealingLevel => CurrentLevelInfo.CurrentRecipesListIndex;

        public float HealHPModifier => CurrentLevelInfo.HealHPModifier;

        public float HealImunnityTicksModifier => CurrentLevelInfo.HealImunnityTicksModifier;

        protected override Dictionary<int, HealingLevel> SkillCharacteristic { get; } = new()
        {
            {1, new HealingLevel(100, 1,0.8f,1)},
            {2, new HealingLevel(125, 2,0.9f,1)},
            {3, new HealingLevel(125, 3,1,1)},
            {4, new HealingLevel(150, 4,1.1f,1)},
            {5, new HealingLevel(150, 5,1.2f,1)},
            {6, new HealingLevel(200, 6,1.4f,1.2f)},
            {7, new HealingLevel(200, 7,1.6f,1.4f)},
            {8, new HealingLevel(300, 8,1.8f,1.5f)},
            {9, new HealingLevel(400, 9,2f,1.8f)},
            {10, new HealingLevel(500,10,2f,2f)}
        };

        protected override void LevelUpped()
        {
            character.Characteristics.HealHpModifier += CurrentLevelInfo.HealHPModifier - PreviousLevelInfo.HealHPModifier;
            character.Characteristics.CurrentHealingLevel += CurrentLevelInfo.CurrentRecipesListIndex - PreviousLevelInfo.CurrentRecipesListIndex;
            character.Characteristics.ImmunityTics += CurrentLevelInfo.HealImunnityTicksModifier - PreviousLevelInfo.HealImunnityTicksModifier;
        }

        public override List<(SerializableSkill, float)> GetLevelInformation()
        {
            return new List<(SerializableSkill, float)>()
            {
                (new SerializableSkill(IconsHelper.Characteristics.HealingLevel, "Уровень оказания перовой помощи"), CurrentHealingLevel)
            };
        }

        public Healing(ICharacter character, int currentLevel = 1, int maxLevel = 10, string name = "Skill", string description = "Description") : base(character, currentLevel, maxLevel, name, description)
        {
        }
    }

    public class HealingLevel : Level
    {
        public int CurrentRecipesListIndex { get; }

        public float HealHPModifier { get; }
        
        public float HealImunnityTicksModifier { get; }

        public HealingLevel(int neededExperienceToLevelUp, int currentRecipesListIndex, float healHpModifier, float healImunnityTicksModifier) : base(neededExperienceToLevelUp)
        {
            CurrentRecipesListIndex = currentRecipesListIndex;
            HealHPModifier = healHpModifier;
            HealImunnityTicksModifier = healImunnityTicksModifier;
        }
    }
}