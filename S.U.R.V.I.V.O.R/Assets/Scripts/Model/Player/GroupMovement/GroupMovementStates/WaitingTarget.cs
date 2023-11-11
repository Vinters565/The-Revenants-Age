using UnityEngine;

namespace TheRevenantsAge
{
    public partial class GroupMovementLogic
    {
        class WaitingTarget : GmState
        {
            public WaitingTarget(GroupMovementLogic gml, StateMachine stateMachine) : base(gml, stateMachine)
            {
            }


            public override void Update()
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                    StateMachine.ChangeState(GroupMovementLogic.sleeping);
                else
                {
                    GroupMovementLogic.DrawPath();
                }

                if (Input.GetMouseButtonDown(0) &&
                    Physics.Raycast(
                        Camera.main.ScreenPointToRay(Input.mousePosition),
                        out var hitInfo, 200f))
                {
                    StateMachine.ChangeState(base.GroupMovementLogic.walking);
                }
            }
        }
    }
}