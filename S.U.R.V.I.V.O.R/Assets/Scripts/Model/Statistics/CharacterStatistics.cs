using System.Runtime.Serialization;

[assembly: ContractNamespace("", ClrNamespace = "")]
namespace TheRevenantsAge
{
    [DataContract]
    public class CharacterStatistics
    {
        [DataMember] public int daysAlive;
        [DataMember] public int lootFound;
        [DataMember] public int monstersKilled;
        [DataMember] public int charactersKilled;
        [DataMember] public float healedHp;
        [DataMember] public float lostHp;
        [DataMember] public int amountOfShots;
        [DataMember] public int energyLost;
        [DataMember] public int foodLost;
        [DataMember] public int waterLost;
    }
}