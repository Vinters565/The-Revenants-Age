using System;

namespace TheRevenantsAge
{
    public abstract class Skill : IDeveloping
    {
        private int maxLevel;
        private int currentLevel;
        private float levelProgress;
        public string Name { get; }
        public string Description { get; }

        public Skill(int maxLevel, int currentLevel = 1, string name = "Skill", string description = "Description")
        {
            Name = name;
            Description = description;
            MaxLevel = maxLevel;
            CurrentLevel = currentLevel;
        }

        public int MaxLevel
        {
            get => maxLevel;
            private set
            {
                maxLevel = Math.Max(1, value);
                currentLevel = Math.Min(MaxLevel, currentLevel);
            }
        }

        public int CurrentLevel
        {
            get => currentLevel;
            protected set => currentLevel = Math.Min(MaxLevel, Math.Max(0, value));
        }

        public float LevelProgress
        {
            get => levelProgress;
            private set => levelProgress = Math.Min(1, Math.Max(0, value));
        }

        public bool IsFinishLevelProgress => Math.Abs(LevelProgress - 1) < 0.000001;

        public virtual void Develop(float levelPercent, Action onLevelUp)
        {
            if (levelPercent < 0) return;
            LevelProgress += levelPercent;
            if (IsFinishLevelProgress)
            {
                LevelProgress = 0;
                CurrentLevel += 1;
                onLevelUp?.Invoke();
            }
        }

        public abstract void Develop(float exp);

        public SkillState CreateState()
        {
            return new SkillState()
            {
                currentLevel = CurrentLevel,
                levelProgress = LevelProgress
            };
        }

        public void Restore(SkillState state)
        {
            if (state == null) return; 
            CurrentLevel = state.currentLevel;
            LevelProgress = state.levelProgress;
        }
    }
}