using System.Runtime.Serialization;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    [DataContract]
    public class EnergyBuff : BaseProperty
    {
        public override string NameProperty => "Бафф энергии";
        public override string DescriptionProperty => "Увеличивает максимальную длину хода на 2";

        public override BasePropertyType BasePropertyType => BasePropertyType.Positive;

        public override void InitialAction(ManBody manBody)
        {
            manBody.MaxOnGlobalMapEndurance += 2;
        }

        public override void TurnEndAction(ManBody manBody)
        {
            if (manBody.MinimalEnergyRecoveryBorder > manBody.Energy)
                manBody.Health.DeleteProperty(this);
        }

        public override void FinalAction(ManBody manBody)
        {
            manBody.MaxOnGlobalMapEndurance -= 2;
        }

        public override IHealthPropertyVisitor Add(IHealthPropertyVisitor healthProperty)
        {
            return this;
        }
    }
}