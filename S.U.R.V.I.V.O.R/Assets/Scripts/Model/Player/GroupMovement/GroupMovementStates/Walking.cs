using Inventory;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public partial class GroupMovementLogic
    {
        class Walking : GmState
        {
            public Walking(GroupMovementLogic gml, StateMachine sm) : base(gml, sm)
            {
            }

            public override void Enter()
            {
                GroupMovementLogic.CreateWay();
                if (GroupMovementLogic.WayLength == 0)
                    StateMachine.ChangeState(GroupMovementLogic.sleeping);
                else
                    GroupMovementLogic.targetNode = GroupMovementLogic.way.Dequeue();
            }

            public override void FixedUpdate()
            {
                GroupMovementLogic.Move();
            }

            public override void Exit()
            {
                LocationInventory.Instance.LocationInventoryGrid.Clear();
                LocationInventory.Instance.LocationInventoryGrid.ResetSizeToInitialize();
            }
        }
    }
}