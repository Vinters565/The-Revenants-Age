using System.Collections.Generic;
using Interface.PlayerInfoLayer;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public class Cooking: ActiveSkill<CookingLevel>
    {
        public float CurrentDishesLevel => CurrentLevelInfo.CurrentRecipesListIndex;
        
        public Cooking(ICharacter character, int currentLevel = 1) : base(character, currentLevel)
        {
        }
        
        protected override Dictionary<int, CookingLevel> SkillCharacteristic { get; } = new()
        {
            {1, new CookingLevel(100, 1)},
            {2, new CookingLevel(125, 2)},
            {3, new CookingLevel(125, 3)},
            {4, new CookingLevel(150, 4)},
            {5, new CookingLevel(150, 5)},
            {6, new CookingLevel(200, 6)},
            {7, new CookingLevel(200, 7)},
            {8, new CookingLevel(300, 8)},
            {9, new CookingLevel(400, 9)},
            {10, new CookingLevel(500,10)}
        };

        protected override void LevelUpped()
        {
            character.Characteristics.CurrentCookingLevel += CurrentLevelInfo.CurrentRecipesListIndex - PreviousLevelInfo.CurrentRecipesListIndex;
        }

        public override List<(SerializableSkill, float)> GetLevelInformation()
        {
            return new List<(SerializableSkill, float)>()
            {
                (new SerializableSkill(IconsHelper.Characteristics.DishesLevel, "Уровень готовки"), CurrentDishesLevel)
            };
        }
    }

    public class CookingLevel : Level
    {
        public int CurrentRecipesListIndex { get; }

        public CookingLevel(int neededExperienceToLevelUp, int currentRecipesListIndex) : base(neededExperienceToLevelUp)
        {
            CurrentRecipesListIndex = currentRecipesListIndex;
        }
    }

}