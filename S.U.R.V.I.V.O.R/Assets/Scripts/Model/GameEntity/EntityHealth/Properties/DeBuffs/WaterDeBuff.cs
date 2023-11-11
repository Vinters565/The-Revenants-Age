using System.Runtime.Serialization;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    [DataContract]
    public class WaterDeBuff : BaseProperty
    {
        public override string NameProperty => "Обезвоживание";

        public override string DescriptionProperty =>
            "Каждый ход убавляет по 35 суммарного хп, уменьшает максимальную длину хода по глобальной карте на 1";

        public override BasePropertyType BasePropertyType => BasePropertyType.Negative;

        public override void InitialAction(ManBody manBody)
        {
            manBody.MaxOnGlobalMapEndurance -= 1;
        }

        public override void TurnEndAction(ManBody manBody)
        {
            manBody.Hp -= 35;
            if (manBody.MinimalWaterDebuffBorder < manBody.Water)
                manBody.Health.DeleteProperty(this);
        }

        public override void FinalAction(ManBody manBody)
        {
            manBody.MaxOnGlobalMapEndurance += 1;
        }
    }
}