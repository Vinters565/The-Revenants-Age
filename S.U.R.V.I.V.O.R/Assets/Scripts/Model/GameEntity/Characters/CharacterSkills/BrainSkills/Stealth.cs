using System.Collections.Generic;
using Interface.PlayerInfoLayer;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public class Stealth: ActiveSkill<StealthLevel>
    {
        public float ExtraNoiceWhileFighting => CurrentLevelInfo.ExtraNociceWhileFighting;
        
        public Stealth(ICharacter character, int currentLevel = 1) : base(character, currentLevel)
        {
        }
        protected override Dictionary<int, StealthLevel> SkillCharacteristic { get; } = new()
        {
            {1, new StealthLevel(100, 20)},
            {2, new StealthLevel(125, 15)},
            {3, new StealthLevel(125, 12)},
            {4, new StealthLevel(150, 10)},
            {5, new StealthLevel(150, 8)},
            {6, new StealthLevel(200, 5)},
            {7, new StealthLevel(200, 2)},
            {8, new StealthLevel(300, 1)},
            {9, new StealthLevel(400, 0)},
            {10, new StealthLevel(500,0)}
        };

        protected override void LevelUpped()
        {
            character.Characteristics.HidingNoice += CurrentLevelInfo.ExtraNociceWhileFighting - PreviousLevelInfo.ExtraNociceWhileFighting;
        }

        public override List<(SerializableSkill, float)> GetLevelInformation()
        {
            return new List<(SerializableSkill, float)>
            {
                (new SerializableSkill(IconsHelper.Characteristics.DeltaNoiseWhileFighting, "Дополнительный шум во время боя"),
                    ExtraNoiceWhileFighting)
            };
        }
    }

    public class StealthLevel : Level
    {
        public int ExtraNociceWhileFighting { get; }

        public StealthLevel(int neededExperienceToLevelUp, int extraNociceWhileFighting) : base(neededExperienceToLevelUp)
        {
            ExtraNociceWhileFighting = extraNociceWhileFighting;
        }
    }
}