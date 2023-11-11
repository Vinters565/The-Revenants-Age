using System.Collections.Generic;
using Interface.PlayerInfoLayer;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public enum HandlingTypes
    {
        Pistol,
        AutoPistol,
        Throwable,
        AssaultRifle,
        SnipeRifle,
        SemiAutoRifle,
        Shotgun,
        ShortMelee,
        LongMelee
    }

    public abstract class GunHandlingSkill : Skill, IDrawableSkill
    {
        protected abstract Dictionary<int, GunHandlingLevel> LevelsDictionary { get; }

        public GunHandlingLevel CurrentHandlingLevel
        {
            get => LevelsDictionary[CurrentLevel];
        }

        protected GunHandlingSkill(int currentLevel = 1) : base(10, currentLevel)
        {
        }

        public override void Develop(float exp)
        {
            base.Develop(exp / CurrentHandlingLevel.NeededExperienceToLevelUp, HandlingUpped);
        }

        private void HandlingUpped()
        {
        }

        public List<(SerializableSkill, float)> GetLevelInformation()
        {
            return new List<(SerializableSkill, float)>()
            {
                (new SerializableSkill(IconsHelper.Characteristics.Accuracy, "Точность"),
                    CurrentHandlingLevel.DeltaAccuracy),
                (new SerializableSkill(IconsHelper.Characteristics.OptimalDistance, "Размер оптимальной дистанции"),
                    CurrentHandlingLevel.DeltaOptimalDistance),
                (new SerializableSkill(IconsHelper.Characteristics.Recoil, "Множитель отдачи"),
                    CurrentHandlingLevel.RecoilModifier),
                (new SerializableSkill(IconsHelper.Characteristics.Ergo, "Эргономика"), 
                    CurrentHandlingLevel.DeltaErgo)
            };
        }

        public int LevelToDraw => (CurrentLevel);
    }
}