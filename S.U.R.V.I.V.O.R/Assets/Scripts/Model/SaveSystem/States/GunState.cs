using System.Runtime.Serialization;

namespace TheRevenantsAge
{
    [DataContract]
    public class GunState : ComponentState
    {
        [DataMember] public ItemState CurrentMagazine;
        [DataMember] public ItemState[] gunModules;
    }
}
