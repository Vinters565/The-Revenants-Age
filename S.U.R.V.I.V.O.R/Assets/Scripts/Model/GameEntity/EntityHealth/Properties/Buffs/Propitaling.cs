using System.Runtime.Serialization;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    [DataContract]
    public class Propitaling : BaseProperty
    {
        public const int PROPITALING_DEFAULT_TURNS_AMOUNT = 5;
        public const int PROPITALING_DEFAULT_HP_AMOUNT = 15;

        private int propitalingHpPerTurn;

        private int propitalingTurnsAmount;

        public Propitaling(int propitalingHpPerTurn, int propitalingTurnsAmount)
        {
            this.propitalingHpPerTurn = propitalingHpPerTurn;
            this.propitalingTurnsAmount = propitalingTurnsAmount;
        }

        public override string NameProperty => "Пропиталинг";

        public override string DescriptionProperty =>
            $"Еще + {propitalingTurnsAmount * propitalingHpPerTurn} ХП, на протяжении {propitalingTurnsAmount} ходов, восстанавливая по {propitalingHpPerTurn} хп в ход. Пропиталинг - это жесточайшее средство шприца пропитал, оно должно восстанавливать 15 хп в ход на протяжении 5 ходов, но тратит одну жажду в ход. Но пропитал давно не производят, поэтому количество добавляемых хп может варьироваться от 10 до 20 в ход. Количество ходов также может варьироваться от 3х до 7ми.";

        public override BasePropertyType BasePropertyType => BasePropertyType.Positive;

        public override void TurnEndAction(ManBody manBody)
        {
            manBody.Hp += propitalingHpPerTurn;
            propitalingTurnsAmount -= 1;
            manBody.Water -= 1;

            if (propitalingTurnsAmount == 0)
                manBody.Health.DeleteProperty(this);
        }

        public override IHealthPropertyVisitor Add(IHealthPropertyVisitor healthProperty)
        {
            return this;
        }
    }
}