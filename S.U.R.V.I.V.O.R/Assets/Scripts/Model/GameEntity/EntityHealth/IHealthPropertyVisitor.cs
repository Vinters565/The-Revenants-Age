using System.Runtime.Serialization;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public interface IHealthPropertyVisitor
    {
        public void InitialActionSwitch(IAlive target)
        {
            switch (target)
            {
                case ManBody manBody: 
                    InitialAction(manBody);
                    break;
                case BodyPart bodyPart:
                    InitialAction(bodyPart);
                    break;
                case Body body:
                    InitialAction(body);
                    break;
                default:
                    InitialAction(target);
                    break;
            }
        }
        
        public void TurnEndActionSwitch(IAlive target)
        {
            switch (target)
            {
                case ManBody manBody:
                    TurnEndAction(manBody);
                    break;
                case BodyPart bodyPart:
                    TurnEndAction(bodyPart);
                    break;
                case Body body:
                    TurnEndAction(body);
                    break;
                default:
                    TurnEndAction(target);
                    break;
            }
        }
        
        
        public void FinalActionSwitch(IAlive target)
        {
            switch (target)
            {
                case ManBody manBody:
                    FinalAction(manBody);
                    break;
                case BodyPart bodyPart:
                    FinalAction(bodyPart);
                    break;
                case Body body:
                    FinalAction(body);
                    break;
                default:
                    FinalAction(target);
                    break;
            }
        }
        
        public void InitialAction(IAlive target);
        public void InitialAction(BodyPart target);
        public void InitialAction(Body target); 
        public void InitialAction(ManBody target);
        
        
        public void TurnEndAction(IAlive target);
        public void TurnEndAction(BodyPart target);
        public void TurnEndAction(Body target); 
        public void TurnEndAction(ManBody target);
        
        
        public void FinalAction(IAlive target);
        public void FinalAction(BodyPart target);
        public void FinalAction(Body target); 
        public void FinalAction(ManBody target);
        
        
        public IHealthPropertyVisitor Add(IHealthPropertyVisitor healthProperty);
    }
}