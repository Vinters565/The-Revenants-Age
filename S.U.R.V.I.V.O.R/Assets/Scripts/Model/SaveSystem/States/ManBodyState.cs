using System.Runtime.Serialization;

namespace TheRevenantsAge
{
    [DataContract]
    public class ManBodyState: BodyState
    {
        [DataMember] public int energy;
        [DataMember] public int hunger;
        [DataMember] public int water;
        [DataMember] public int onGlobalMapEndurance;

        [DataMember] public int maxEnergy;
        [DataMember] public int maxHunger;
        [DataMember] public int maxWater;
        [DataMember] public int maxOnGlobalMapEndurance;
    }
}