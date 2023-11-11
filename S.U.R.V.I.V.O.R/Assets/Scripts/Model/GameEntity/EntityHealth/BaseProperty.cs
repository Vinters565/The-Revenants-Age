using System.Runtime.Serialization;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public enum BasePropertyType
    {
        Positive,
        Negative,
        None
    }
    [DataContract]
    public abstract class BaseProperty : IHealthPropertyVisitor
    {
        public virtual BasePropertyType BasePropertyType => BasePropertyType.None;
        public virtual string NameProperty => "Название по умолчанию";
        public virtual string DescriptionProperty => "Описание по умолчанию";


        public virtual void InitialAction(IAlive target) {}
        public virtual void InitialAction(BodyPart target) {}
        public virtual void InitialAction(Body target) {}
        public virtual void InitialAction(ManBody target) {}
        

        public virtual void TurnEndAction(IAlive target) {}
        public virtual void TurnEndAction(BodyPart target){}
        public virtual void TurnEndAction(Body target) {}
        public virtual void TurnEndAction(ManBody target) {}
        

        public virtual void FinalAction(IAlive target) {}
        public virtual void FinalAction(BodyPart target) {}
        public virtual void FinalAction(Body target) {}
        public virtual void FinalAction(ManBody target) {}

        public virtual IHealthPropertyVisitor Add(IHealthPropertyVisitor healthProperty) => this;
    }
}