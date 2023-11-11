using System;

namespace TheRevenantsAge
{
    public class GunHandlingLevel
    {
        public GunHandlingLevel(float neededExperienceToLevelUp, float recoilModifier, float deltaOptimalDistance, float deltaErgo, short maxAmountOfShotsOnSemiAuto, bool isReloadFree, float deltaAccuracy)
        {
            NeededExperienceToLevelUp = neededExperienceToLevelUp;
            RecoilModifier = recoilModifier;
            DeltaOptimalDistance = deltaOptimalDistance;
            DeltaErgo = deltaErgo;
            MaxAmountOfShotsOnSemiAuto = maxAmountOfShotsOnSemiAuto;
            IsReloadFree = isReloadFree;
            DeltaAccuracy = deltaAccuracy;
        }

        public float NeededExperienceToLevelUp { get; }
        public float DeltaAccuracy { get; }
        public float RecoilModifier { get; }
        public float DeltaOptimalDistance { get; }
        public float DeltaErgo { get; }
        public Int16 MaxAmountOfShotsOnSemiAuto { get; }
        public bool IsReloadFree { get; }
    }
}
