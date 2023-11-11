using System.Runtime.Serialization;

namespace TheRevenantsAge
{
    [DataContract]
    public class ClothesState: ComponentState
    {
        [DataMember] public float currentArmor;
        [DataMember] public ItemState[] inventory;

        public override void CopyFromOldState(ComponentState oldState)
        {
            if (oldState is ClothesState oldClothesState)
                this.inventory = oldClothesState.inventory;
        }
    }
}