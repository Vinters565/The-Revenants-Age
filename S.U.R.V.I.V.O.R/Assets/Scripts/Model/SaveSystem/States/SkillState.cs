using System.Runtime.Serialization;

namespace TheRevenantsAge
{
    [DataContract]
    public class SkillState
    {
        [DataMember] public int currentLevel;
        [DataMember] public float levelProgress;
    }
}