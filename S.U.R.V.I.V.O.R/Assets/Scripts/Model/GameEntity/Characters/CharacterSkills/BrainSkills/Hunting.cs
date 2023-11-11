using System.Collections.Generic;
using Interface.PlayerInfoLayer;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public class Hunting : ActiveSkill<HuntingLevel>
    {
        public float ChanceToHunt => CurrentLevelInfo.ChanceToHunt;

        public Hunting(ICharacter character, int currentLevel = 1) : base(character, currentLevel)
        {
        }
        protected override Dictionary<int, HuntingLevel> SkillCharacteristic { get; } = new()
        {
            { 1, new HuntingLevel(100, 0.1f) },
            { 2, new HuntingLevel(125, 0.2f) },
            { 3, new HuntingLevel(125, 0.3f) },
            { 4, new HuntingLevel(150, 0.4f) },
            { 5, new HuntingLevel(150, 0.5f) },
            { 6, new HuntingLevel(200, 0.6f) },
            { 7, new HuntingLevel(200, 0.7f) },
            { 8, new HuntingLevel(300, 0.8f) },
            { 9, new HuntingLevel(400, 0.9f) },
            { 10, new HuntingLevel(500, 1f) }
        };

        protected override void LevelUpped()
        {
            character.Characteristics.ChanceToHunt += CurrentLevelInfo.ChanceToHunt - PreviousLevelInfo.ChanceToHunt;
        }

        public override List<(SerializableSkill, float)> GetLevelInformation()
        {
            return new List<(SerializableSkill, float)>()
            {
                (new SerializableSkill(IconsHelper.Characteristics.ChanceToCutFish, "Шанс удачной охоты"), ChanceToHunt)
            };
        }
    }

    public class HuntingLevel : Level
    {
        public float ChanceToHunt { get; }

        public HuntingLevel(int neededExperienceToLevelUp, float chanceToHunt) : base(
            neededExperienceToLevelUp)
        {
            ChanceToHunt = chanceToHunt;
        }
    }
}