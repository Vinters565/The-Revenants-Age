using System.Collections.Generic;
using Interface.PlayerInfoLayer;

namespace TheRevenantsAge
{
    public class Accuracy: ActiveSkill<AccuracyLevel>
    {
        public float DeltaAccuracy => CurrentLevelInfo.DeltaAccuracy;
        public float RecoilModifier => CurrentLevelInfo.RecoilModifier;
        
        public Accuracy(ICharacter character, int currentLevel = 1) : base(character, currentLevel, name: "Точность")
        { }

        protected override Dictionary<int, AccuracyLevel> SkillCharacteristic { get; } = new()
        {
            {1, new AccuracyLevel(100, 0,1)},
            {2, new AccuracyLevel(125, 0.05f,0.95f)},
            {3, new AccuracyLevel(125, 0.1f,0.95f)},
            {4, new AccuracyLevel(150, 0.1f,0.9f)},
            {5, new AccuracyLevel(150, 0.15f,0.9f)},
            {6, new AccuracyLevel(200, 0.15f,0.85f)},
            {7, new AccuracyLevel(200, 0.15f,0.8f)},
            {8, new AccuracyLevel(300, 0.2f,0.8f)},
            {9, new AccuracyLevel(400, 0.2f,0.75f)},
            {10, new AccuracyLevel(500,0.25f,0.75f)}
        };

        protected override void LevelUpped()
        {
            character.Characteristics.Accuracy += CurrentLevelInfo.DeltaAccuracy - PreviousLevelInfo.DeltaAccuracy;
            character.Characteristics.RecoilModifier +=  CurrentLevelInfo.RecoilModifier - PreviousLevelInfo.RecoilModifier;
        }

        public override List<(SerializableSkill, float)> GetLevelInformation()
        {
            return new List<(SerializableSkill, float)>()
            {
                (new SerializableSkill(IconsHelper.Characteristics.Accuracy, "Точность"), DeltaAccuracy),
                (new SerializableSkill(IconsHelper.Characteristics.Recoil, "Множитель отдачи"), RecoilModifier),
            };
        }
    }

    public class AccuracyLevel : Level
    {
        public float DeltaAccuracy { get; }
        
        public float RecoilModifier { get; }

        public AccuracyLevel(int neededExperienceToLevelUp, float deltaAccuracy, float recoilModifier) : base(neededExperienceToLevelUp)
        {
            DeltaAccuracy = deltaAccuracy;
            RecoilModifier = recoilModifier;
        }
    }
}