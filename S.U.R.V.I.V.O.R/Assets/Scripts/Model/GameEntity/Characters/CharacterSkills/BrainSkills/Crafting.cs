using System.Collections.Generic;
using Interface.PlayerInfoLayer;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public class Crafting: ActiveSkill<CraftingLevel>
    {
        public float CurrentCraftingLevel => CurrentLevelInfo.CurrentRecipesListIndex;
        
        public Crafting(ICharacter character, int currentLevel = 1) : base(character, currentLevel)
        {
        }

        protected override Dictionary<int, CraftingLevel> SkillCharacteristic { get; } = new()
        {
            {1, new CraftingLevel(100, 1)},
            {2, new CraftingLevel(125, 2)},
            {3, new CraftingLevel(125, 3)},
            {4, new CraftingLevel(150, 4)},
            {5, new CraftingLevel(150, 5)},
            {6, new CraftingLevel(200, 6)},
            {7, new CraftingLevel(200, 7)},
            {8, new CraftingLevel(300, 8)},
            {9, new CraftingLevel(400, 9)},
            {10, new CraftingLevel(500,10)}
        };

        protected override void LevelUpped()
        {
            character.Characteristics.CurrentCraftLevel += CurrentLevelInfo.CurrentRecipesListIndex - PreviousLevelInfo.CurrentRecipesListIndex;
        }

        public override List<(SerializableSkill, float)> GetLevelInformation()
        {
            return new List<(SerializableSkill, float)>()
            {
                (new SerializableSkill(IconsHelper.Characteristics.CraftingLevel, "Уровень создания предметов"), CurrentCraftingLevel)
            };
        }


    }

    public class CraftingLevel : Level
    {
        public int CurrentRecipesListIndex { get; }

        public CraftingLevel(int neededExperienceToLevelUp, int currentRecipesListIndex) : base(neededExperienceToLevelUp)
        {
            CurrentRecipesListIndex = currentRecipesListIndex;
        }
    }
}