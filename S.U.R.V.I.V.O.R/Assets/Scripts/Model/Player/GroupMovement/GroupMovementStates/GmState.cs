namespace TheRevenantsAge
{
    public partial class GroupMovementLogic
    {
        abstract class GmState : State
        {
            protected readonly GroupMovementLogic GroupMovementLogic;

            protected GmState(GroupMovementLogic groupMovementLogic, StateMachine stateMachine)
            {
                GroupMovementLogic = groupMovementLogic;
                StateMachine = stateMachine;
            }

        }
    }
}