using System.Collections.Generic;
using Interface.PlayerInfoLayer;

namespace TheRevenantsAge
{
    public class Charisma: ActiveSkill<CharismaLevel>
    {
        public Charisma(ICharacter character, int currentLevel = 1) : base(character, currentLevel, name: "Харизма")
        {
        }

        protected override Dictionary<int, CharismaLevel> SkillCharacteristic { get; } = new()
        {
            {1, new CharismaLevel(100, 1)},
            {2, new CharismaLevel(125, 1)},
            {3, new CharismaLevel(125, 1)},
            {4, new CharismaLevel(150, 2)},
            {5, new CharismaLevel(150, 2)},
            {6, new CharismaLevel(200, 2)},
            {7, new CharismaLevel(200, 3)},
            {8, new CharismaLevel(300, 3)},
            {9, new CharismaLevel(400, 3)},
            {10, new CharismaLevel(500,4)}
        };
        

        protected override void LevelUpped()
        {
            character.Characteristics.MaxPeopleInGroup += CurrentLevelInfo.MaxPeopleInGroup - PreviousLevelInfo.MaxPeopleInGroup;
        }

        public override List<(SerializableSkill, float)> GetLevelInformation()
        {
            return new List<(SerializableSkill, float)>
            {
                (new SerializableSkill(IconsHelper.Characteristics.MaxPeopleInGroup, "Максимальное число людей в группе"), SkillCharacteristic[CurrentLevel].MaxPeopleInGroup)
            };
            
        }
    }

    public class CharismaLevel : Level
    {
        public int MaxPeopleInGroup { get; }

        public CharismaLevel(int neededExperienceToLevelUp, int maxPeopleInGroup):base(neededExperienceToLevelUp)
        {
            MaxPeopleInGroup = maxPeopleInGroup;
        }
    }
}