using System.Collections.Generic;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public sealed class Warmbrodeed : ActiveSkill<WarmbrodeedLevel>
    {
        public Warmbrodeed(ICharacter character, int currentLevel = 1) : base(character, currentLevel)
        { }

        protected override Dictionary<int, WarmbrodeedLevel> SkillCharacteristic { get; } = new()
        {
            {1, new WarmbrodeedLevel(100, 0)},
            {2, new WarmbrodeedLevel(125, 1)},
            {3, new WarmbrodeedLevel(125, 2)},
            {4, new WarmbrodeedLevel(150, 3)},
            {5, new WarmbrodeedLevel(150, 4)},
            {6, new WarmbrodeedLevel(200, 5)},
            {7, new WarmbrodeedLevel(200, 6)},
            {8, new WarmbrodeedLevel(300, 7)},
            {9, new WarmbrodeedLevel(400, 8)},
            {10, new WarmbrodeedLevel(500,10)}
        };

        protected override void LevelUpped()
        {
            character.Characteristics.ColdResistance += CurrentLevelInfo.ExtraColdResistance - PreviousLevelInfo.ExtraColdResistance;
        }
        
    }
    
    public class WarmbrodeedLevel : Level
    {
        public int ExtraColdResistance { get; }

        public WarmbrodeedLevel(int neededExperienceToLevelUp, int extraColdResistance):base(neededExperienceToLevelUp)
        {
            ExtraColdResistance = extraColdResistance;
        }
    }
}