using System;
using System.Runtime.Serialization;

namespace TheRevenantsAge
{
    [DataContract]
    public class Bleeding : BaseProperty
    {
        [DataMember] private int turnsToAutoHeal = 4;
        public override string NameProperty => "Кровотечение";
        public override string DescriptionProperty => "Какое-то описание";
        
        public override void TurnEndAction(BodyPart bodyPart)
        {
            bodyPart.Hp -= 5;
            turnsToAutoHeal -= 1;
            if (turnsToAutoHeal == 0)
                bodyPart.Health.DeleteProperty(this);
        }

        public override IHealthPropertyVisitor Add(IHealthPropertyVisitor healthProperty)
        {
            if (healthProperty is Bleeding prop)
            {
                turnsToAutoHeal += prop.turnsToAutoHeal;
                return this;
            }
            throw new InvalidOperationException();
        }
    }
}