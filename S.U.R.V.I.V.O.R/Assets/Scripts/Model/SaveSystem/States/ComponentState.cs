using System.Runtime.Serialization;

namespace TheRevenantsAge
{
    [DataContract]
    public abstract class ComponentState
    {
        [DataMember] public ItemState itemState;
        
        public virtual void CopyFromOldState(ComponentState oldState) {}
    }
}