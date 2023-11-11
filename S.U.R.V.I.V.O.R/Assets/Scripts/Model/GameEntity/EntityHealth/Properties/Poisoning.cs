using System;
using System.Runtime.Serialization;


namespace TheRevenantsAge
{
    [DataContract]
    public class Poisoning : BaseProperty
    {
        private int turnsToAutoHeal = 6;

        public override string NameProperty => "Отравление";
        public override string DescriptionProperty => "Какое-то описание";
        
        public override void TurnEndAction(IAlive target)
        {
            target.Hp -= 10;
            turnsToAutoHeal -= 1;
            if (turnsToAutoHeal == 0)
            {
                target.Health.DeleteProperty(this);
            }
        }
        

        public override IHealthPropertyVisitor Add(IHealthPropertyVisitor healthProperty)
        {
            if (healthProperty is Poisoning prop)
            {
                turnsToAutoHeal += prop.turnsToAutoHeal;
                return this;
            }
            throw new InvalidOperationException();
        }
    }
}