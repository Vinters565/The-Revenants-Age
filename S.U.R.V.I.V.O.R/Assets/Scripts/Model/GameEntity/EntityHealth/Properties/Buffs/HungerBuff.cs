using System.Runtime.Serialization;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    [DataContract]
    public class HungerBuff : BaseProperty
    {
        public override string NameProperty => "Сытость";
        public override string DescriptionProperty => "Каждый ход восстанавливает по 10 хп и 1 энергию";

        public override BasePropertyType BasePropertyType => BasePropertyType.Positive;

        public override void TurnEndAction(ManBody manBody)
        {
            manBody.Energy += 1;
            manBody.Hp += 10;
            if (manBody.MinimalHungerRecoveryBorder > manBody.Hunger)
            {
                manBody.Health.DeleteProperty(this);
            }
        }
    }
}