using System.Collections.Generic;
using Interface.PlayerInfoLayer;

namespace TheRevenantsAge
{
    public abstract class PassiveSkill<T> : Skill, IDrawableSkill where T: Level
    {
        protected abstract Dictionary<int, T> SkillCharacteristic { get; }
        
        protected T CurrentLevelInfo => SkillCharacteristic[CurrentLevel];

        public override void Develop(float exp)
        {
            base.Develop(exp/CurrentLevelInfo.NeededExperienceToLevelUp, LevelUpped);
        }
        protected PassiveSkill(int currentLevel = 1, int maxLevel = 10, string name = "Skill", string description = "Description") : base(maxLevel, currentLevel,name,description)
        { }

        private void LevelUpped()
        { }

        public virtual List<(SerializableSkill, float)> GetLevelInformation()
        {
            return null;//TODO ЗАТЫЧКА
        }

        public int LevelToDraw => CurrentLevel;
    }
}