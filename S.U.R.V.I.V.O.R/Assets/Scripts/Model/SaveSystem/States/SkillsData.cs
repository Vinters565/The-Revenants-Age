using System.Runtime.Serialization;

namespace TheRevenantsAge
{
    [DataContract]
    public class SkillsData
    {
        [DataMember] public SkillState strength;
        [DataMember] public SkillState durability;
        [DataMember] public SkillState survival;
        [DataMember] public SkillState warmbrodeed;

        [DataMember]public SkillState charisma;
        [DataMember]public SkillState healing;
        [DataMember]public SkillState crafting;
        [DataMember]public SkillState cooking;
        [DataMember]public SkillState looting;
        [DataMember]public SkillState accuracy;
        [DataMember]public SkillState hunting;
        [DataMember]public SkillState fishing;
        [DataMember]public SkillState stealth;

        [DataMember] public SkillState pistolHandlingSkill;
        [DataMember] public SkillState autoPistolHandlingSkill;
        [DataMember] public SkillState throwableHandlingSkill;
        [DataMember] public SkillState assaultRifleHandlingSkill;
        [DataMember] public SkillState snipeRifleHandlingSkill;
        [DataMember] public SkillState semiAutoRifleHandlingSkill;
        [DataMember] public SkillState shotgunHandlingSkill;
        [DataMember] public SkillState shortMeleeHandlingSkill;
        [DataMember] public SkillState longMeleeHandlingSkill;
    }
}