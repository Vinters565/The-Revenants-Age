using System.Runtime.Serialization;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    [DataContract]
    public class EnergyDeBuff : BaseProperty
    {
        public override string NameProperty => "Недостаток энергии";
        public override string DescriptionProperty => "Уменьшает максимальную длину хода на 4";

        public override BasePropertyType BasePropertyType => BasePropertyType.Negative;

        public override void InitialAction(ManBody manBody)
        {
            manBody.MaxOnGlobalMapEndurance -= 4;
        }

        public override void TurnEndAction(ManBody manBody)
        {
            if (manBody.MinimalEnergyDebuffBorder < manBody.Energy)
                manBody.Health.DeleteProperty(this);
        }

        public override void FinalAction(ManBody manBody)
        {
            manBody.MaxOnGlobalMapEndurance += 4;
        }
    }
}