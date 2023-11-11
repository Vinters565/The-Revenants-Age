using System.Collections.Generic;
using Interface.PlayerInfoLayer;

namespace TheRevenantsAge
{
    public interface IDrawableSkill
    {
        public List<(SerializableSkill,float)> GetLevelInformation();

        public int LevelToDraw { get; }
    }
}