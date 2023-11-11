using System.Runtime.Serialization;

namespace TheRevenantsAge
{
    [DataContract]
    public class GroupStatistics
    {
        [DataMember] public CharacterStatistics[] characterStatisticsArray;

        public GroupStatistics(CharacterStatistics[] characterStatisticsArray)
        {
            this.characterStatisticsArray = characterStatisticsArray;
        }
    }
}