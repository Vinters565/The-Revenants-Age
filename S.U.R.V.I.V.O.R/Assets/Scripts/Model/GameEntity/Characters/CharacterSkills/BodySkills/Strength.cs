using System.Collections.Generic;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public sealed class Strength : ActiveSkill<StrengthLevel>
    {
        public Strength(ICharacter character, int currentLevel = 1) : base(character, currentLevel)
        { }
        protected override Dictionary<int, StrengthLevel> SkillCharacteristic { get; } = new()
        {
            {1, new StrengthLevel(100, 0, 0, 0)},
            {2, new StrengthLevel(125, 0, 2, 2)},
            {3, new StrengthLevel(125, 3, 2, 5)},
            {4, new StrengthLevel(150, 5, 5, 5)},
            {5, new StrengthLevel(150, 5, 7, 5)},
            {6, new StrengthLevel(200, 7, 10, 7)},
            {7, new StrengthLevel(200, 10, 12, 10)},
            {8, new StrengthLevel(300, 10, 15, 15)},
            {9, new StrengthLevel(400, 15, 17, 20)},
            {10, new StrengthLevel(500,20, 20, 25)}
        };

        protected override void LevelUpped()
        {
            foreach (var bodyPart in character.ManBody.BodyParts)
            {
                bodyPart.MaxHp += CurrentLevelInfo.AdditionalHp - PreviousLevelInfo.AdditionalHp;
            }
            character.Characteristics.MeleeDamage += CurrentLevelInfo.AdditionalMeleeDamage - PreviousLevelInfo.AdditionalMeleeDamage;
            character.Body.MaxWeightToGrab += CurrentLevelInfo.AdditionalPortableWeight - PreviousLevelInfo.AdditionalPortableWeight;
        }
    }
    
    public class StrengthLevel : Level
    {
        public int AdditionalHp { get; }
        public int AdditionalPortableWeight { get; }
        public int AdditionalMeleeDamage { get; }

        public StrengthLevel(int neededExperienceToLevelUp, int additionalHp, int additionalPortableWeight,
            int additionalMeleeDamage):base(neededExperienceToLevelUp)
        {
            AdditionalHp = additionalHp;
            AdditionalPortableWeight = additionalPortableWeight;
            AdditionalMeleeDamage = additionalMeleeDamage;
        }
    }
}