using System.Collections.Generic;
using Interface.PlayerInfoLayer;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public class Fishing : ActiveSkill<FishingLevel>
    {
        public float ChanceToCutFish => CurrentLevelInfo.ChanceToCutFish;

        protected override Dictionary<int, FishingLevel> SkillCharacteristic { get; } = new()
        {
            { 1, new FishingLevel(100, 0.1f) },
            { 2, new FishingLevel(125, 0.2f) },
            { 3, new FishingLevel(125, 0.3f) },
            { 4, new FishingLevel(150, 0.4f) },
            { 5, new FishingLevel(150, 0.5f) },
            { 6, new FishingLevel(200, 0.6f) },
            { 7, new FishingLevel(200, 0.7f) },
            { 8, new FishingLevel(300, 0.8f) },
            { 9, new FishingLevel(400, 0.9f) },
            { 10, new FishingLevel(500, 1f) }
        };

        protected override void LevelUpped()
        {
            character.Characteristics.ChanceToCutFish += CurrentLevelInfo.ChanceToCutFish - PreviousLevelInfo.ChanceToCutFish;
        }

        public override List<(SerializableSkill, float)> GetLevelInformation()
        {
            return new List<(SerializableSkill, float)>()
            {
                (new SerializableSkill(IconsHelper.Characteristics.ChanceToCutFish, "Шанс поймать рыбу"), ChanceToCutFish)
            };
        }

        public Fishing(ICharacter character, int currentLevel = 1, int maxLevel = 10, string name = "Skill", string description = "Description") : base(character, currentLevel, maxLevel, name, description)
        {
        }
    }

    public class FishingLevel : Level
    {
        public float ChanceToCutFish { get; }

        public FishingLevel(int neededExperienceToLevelUp, float chanceToCutFish) : base(
            neededExperienceToLevelUp)
        {
            ChanceToCutFish = chanceToCutFish;
        }
    }
}