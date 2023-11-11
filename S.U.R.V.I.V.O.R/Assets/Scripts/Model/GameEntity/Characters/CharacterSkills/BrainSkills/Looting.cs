using System.Collections.Generic;
using Interface.PlayerInfoLayer;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public class Looting: ActiveSkill<LootingLevel>
    {
        public float ChanceToFindExtraItem => CurrentLevelInfo.ChanceToFindExtraItem;

        public float DeltaNoiceWhileLooting => CurrentLevelInfo.DeltaNoiceWhileLooting;
        public bool CanFreeLoot => CurrentLevelInfo.CanFreeLoot;
        

        protected override Dictionary<int, LootingLevel> SkillCharacteristic { get; } = new()
        {
            {1, new LootingLevel(100, 0,10,false)},
            {2, new LootingLevel(125, 0,9,false)},
            {3, new LootingLevel(125, 0.02f,8,false)},
            {4, new LootingLevel(150, 0.05f,7,false)},
            {5, new LootingLevel(150, 0.07f,6,false)},
            {6, new LootingLevel(200, 0.1f,5,false)},
            {7, new LootingLevel(200, 0.13f,4,false)},
            {8, new LootingLevel(300, 0.15f,3,true)},
            {9, new LootingLevel(400, 0.17f,2,true)},
            {10, new LootingLevel(500,0.2f,1,true)}
        };

        protected override void LevelUpped()
        {
            character.Characteristics.ChanceToFindExtraItem += CurrentLevelInfo.ChanceToFindExtraItem - PreviousLevelInfo.ChanceToFindExtraItem;
            character.Characteristics.NoiceWhileLooting += CurrentLevelInfo.DeltaNoiceWhileLooting - PreviousLevelInfo.DeltaNoiceWhileLooting;
            character.Characteristics.CanFreeLoot = CurrentLevelInfo.CanFreeLoot;
        }

        public override List<(SerializableSkill, float)> GetLevelInformation()
        {
            return new List<(SerializableSkill, float)>
            {
                (new SerializableSkill(IconsHelper.Characteristics.ChanceToFindExtraItem, "Шанс найти дополнительные предметы"), ChanceToFindExtraItem),
                (new SerializableSkill(IconsHelper.Characteristics.DeltaNoiseWhileLooting, "Дополнительный шум при поиске лута"), DeltaNoiceWhileLooting)
            };
        }

        public Looting(ICharacter character, int currentLevel = 1, int maxLevel = 10, string name = "Skill", string description = "Description") : base(character, currentLevel, maxLevel, name, description)
        {
        }
    }
    
    public class LootingLevel : Level
    {
        public float ChanceToFindExtraItem { get; }
        
        public float DeltaNoiceWhileLooting { get; }
        
        public bool CanFreeLoot { get; }

        public LootingLevel(int neededExperienceToLevelUp, float chanceToFindExtraItem, float deltaNoiceWhileLooting, bool canFreeLoot) : base(neededExperienceToLevelUp)
        {
            ChanceToFindExtraItem = chanceToFindExtraItem;
            DeltaNoiceWhileLooting = deltaNoiceWhileLooting;
            CanFreeLoot = canFreeLoot;
        }
    }
}