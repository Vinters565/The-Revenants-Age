using System;
using System.Runtime.Serialization;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    [DataContract]
    public class Broking : BaseProperty
    {
        [DataMember] private int turnsToAutoHeal;

        public override string NameProperty => "Что-то сломано";
        public override string DescriptionProperty => "Ай ай яй, поломался. Ну как так то.";

        public override void InitialAction(ManBody target)
        {
            base.InitialAction(target);
            target.MaxOnGlobalMapEndurance -= 1;
        }
        
        
        public override void TurnEndAction(BodyPart target)
        {
           target.Hp -= 3;
            turnsToAutoHeal -= 1;
            if (turnsToAutoHeal == 0)
            {
                target.Health.DeleteProperty(this);
            }
        }

        public override void FinalAction(ManBody manBody)
        {
            manBody.MaxOnGlobalMapEndurance += 1;
        }

        public override IHealthPropertyVisitor Add(IHealthPropertyVisitor healthProperty)
        {
            if (healthProperty is Broking prop)
            {
                turnsToAutoHeal += prop.turnsToAutoHeal;
                return this;
            }
            throw new InvalidOperationException();
        }
    }
}