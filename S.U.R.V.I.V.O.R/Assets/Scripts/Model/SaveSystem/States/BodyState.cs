using System.Runtime.Serialization;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    [DataContract]
    public class BodyState
    {
        [DataMember] public IHealthPropertyVisitor[] healthProperties;
        [DataMember] public int currentCriticalLoses;
        [DataMember] public BodyPartState[] bodyPartSaves;
    }
}