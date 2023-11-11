using System.Runtime.Serialization;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    [DataContract]
    public class WaterBuff : BaseProperty
    {
        public override string NameProperty => "Насыщение водой";
        public override string DescriptionProperty => "Каждый ход восстанавливает по 10 хп и одну энергию.";

        public override BasePropertyType BasePropertyType => BasePropertyType.Positive;

        public override void TurnEndAction(ManBody manBody)
        {
            manBody.Energy += 1;
            manBody.Hp += 10;
            if (manBody.MinimalWaterRecoveryBorder > manBody.Water)
            {
                manBody.Health.DeleteProperty(this);
            }
        }
    }
}