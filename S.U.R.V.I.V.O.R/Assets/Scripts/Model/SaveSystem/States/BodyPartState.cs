using System.Runtime.Serialization;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    [DataContract]
    public class BodyPartState
    {
        [DataMember] public IHealthPropertyVisitor[] healthProperties;
        [DataMember] public float maxHp;
        [DataMember] public float hp;
        [DataMember] public float size;
    }
}