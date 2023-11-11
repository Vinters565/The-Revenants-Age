using System.Collections.Generic;
using Interface.PlayerInfoLayer;

namespace TheRevenantsAge
{
    public abstract class ActiveSkill<T> : Skill, IDrawableSkill where T: Level
    {
        protected abstract Dictionary<int, T> SkillCharacteristic { get; }
        
        protected readonly ICharacter character;
        protected T CurrentLevelInfo => SkillCharacteristic[CurrentLevel];
        
        protected T PreviousLevelInfo => SkillCharacteristic[CurrentLevel - 1];

        public override void Develop(float exp)
        {
            base.Develop(exp/CurrentLevelInfo.NeededExperienceToLevelUp, LevelUpped);
        }
        protected ActiveSkill(ICharacter character, int currentLevel = 1, int maxLevel = 10, string name = "Skill", string description = "Description") : base(maxLevel, currentLevel, name, description)
        {
            this.character = character;
        }

        protected abstract void LevelUpped();

        public virtual List<(SerializableSkill, float)> GetLevelInformation()
        {
            return null;
        }
        
        public int LevelToDraw => CurrentLevel;
    }
}