namespace TheRevenantsAge
{
    public partial class GroupMovementLogic
    {
        class Sleeping : GmState
        {
            public Sleeping(GroupMovementLogic groupMovementLogic, StateMachine sm) : base(groupMovementLogic, sm)
            {
            }

            public override void Enter()
            {
                GroupMovementLogic.ClearWay();
                
            }
        }
    }
}