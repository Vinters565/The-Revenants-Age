using System.Collections.Generic;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public sealed class Durability: ActiveSkill<DurabilityLevel>
    {
        public Durability(ICharacter character, int maxLevel, int currentLevel = 1) : base(character, maxLevel, currentLevel)
        { }

        protected override Dictionary<int, DurabilityLevel> SkillCharacteristic { get; } = new()
        {
            { 1, new DurabilityLevel(100, 0, 0, 0,0) },
            { 2, new DurabilityLevel(100, 0, 0, 0,2) },
            { 3, new DurabilityLevel(100, 1, 0, 0,5) },
            { 4, new DurabilityLevel(100, 1, 0, 0,7) },
            { 5, new DurabilityLevel(100, 1, 1, 0,10) },
            { 6, new DurabilityLevel(100, 1, 1, 0,13) },
            { 7, new DurabilityLevel(100, 2, 1, 0,15) },
            { 8, new DurabilityLevel(100, 2, 1, 0,20) },
            { 9, new DurabilityLevel(100, 2, 1, 0,20) },
            { 10, new DurabilityLevel(100, 3, 2, 1,25) }
        };

        protected override void LevelUpped()
        {
            character.ManBody.MaxOnGlobalMapEndurance += CurrentLevelInfo.AdditionalOnGlobalMapEndurance -
                                                         PreviousLevelInfo.AdditionalOnGlobalMapEndurance;
            
            character.Characteristics.PointsOfTurn += CurrentLevelInfo.AdditionalPointsOfTurn -
                                    PreviousLevelInfo.AdditionalPointsOfTurn;
            
            character.Characteristics.PointsOfAction += CurrentLevelInfo.AdditionalPointsOfAction -
                                      PreviousLevelInfo.AdditionalPointsOfAction;
            
            character.Characteristics.Initiative += CurrentLevelInfo.AdditionalInitiative -
                                    PreviousLevelInfo.AdditionalInitiative;
        }
    }

    public class DurabilityLevel : Level
    {
        public int AdditionalOnGlobalMapEndurance { get; }
        public int AdditionalPointsOfAction { get; }    
        public int AdditionalPointsOfTurn { get; }
        public int AdditionalInitiative { get; }

        public DurabilityLevel(int neededExperienceToLevelUp, int additionalOnGlobalMapEndurance,
            int additionalPointsOfAction, int additionalPointsOfTurn,
            int additionalInitiative) : base(neededExperienceToLevelUp)
        {
            AdditionalOnGlobalMapEndurance = additionalOnGlobalMapEndurance;
            AdditionalPointsOfAction = additionalPointsOfAction;
            AdditionalPointsOfTurn = additionalPointsOfTurn;
            AdditionalInitiative = additionalInitiative;
        }
    }
}