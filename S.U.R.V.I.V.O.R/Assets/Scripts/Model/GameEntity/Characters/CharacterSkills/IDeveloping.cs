using System;

namespace TheRevenantsAge
{
    public interface IDeveloping
    {
        public int CurrentLevel { get; }
        public void Develop(float levelPercent, Action onLevelUp);
    }
}