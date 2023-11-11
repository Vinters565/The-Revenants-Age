using System.Runtime.Serialization;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    [DataContract]
    public class HungerDeBuff : BaseProperty
    {
        public override string NameProperty => "Голод";

        public override string DescriptionProperty =>
            "Каждый ход убавляет по 30 суммарного хп, уменьшает максимальную длину хода по глобальной карте на 3";

        public override BasePropertyType BasePropertyType => BasePropertyType.Negative;

        public override void InitialAction(ManBody manBody)
        {
            manBody.MaxOnGlobalMapEndurance -= 3;
        }

        public override void TurnEndAction(ManBody manBody)
        {
            manBody.Hp -= 30;
            if (manBody.MinimalHungerDebuffBorder < manBody.Hunger)
            {
                manBody.Health.DeleteProperty(this);
            }
        }

        public override void FinalAction(ManBody manBody)
        {
            manBody.MaxOnGlobalMapEndurance += 3;
        }
    }
}