using System.Collections.Generic;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public sealed class Survival : ActiveSkill<SurvivalLevel>
    {
        public Survival(ICharacter character, int currentLevel = 1) : base(character, currentLevel)
        { }
        protected override Dictionary<int, SurvivalLevel> SkillCharacteristic { get; } = new()
        {
            {1, new SurvivalLevel(100, 5)},
            {2, new SurvivalLevel(125, 7)},
            {3, new SurvivalLevel(125, 10)},
            {4, new SurvivalLevel(150, 13)},
            {5, new SurvivalLevel(150, 15)},
            {6, new SurvivalLevel(200, 20)},
            {7, new SurvivalLevel(200, 23)},
            {8, new SurvivalLevel(300, 25)},
            {9, new SurvivalLevel(400, 30)},
            {10, new SurvivalLevel(500,40)}
        };

        protected override void LevelUpped()
        {
            character.Characteristics.ImmunityTics += CurrentLevelInfo.ExtraImunnityTicks - PreviousLevelInfo.ExtraImunnityTicks;
        }
    }
    
    public class SurvivalLevel : Level
    {
        public int ExtraImunnityTicks { get; }

        public SurvivalLevel(int neededExperienceToLevelUp, int extraImunnityTicks):base(neededExperienceToLevelUp)
        {
            ExtraImunnityTicks = extraImunnityTicks;
        }
    }
}