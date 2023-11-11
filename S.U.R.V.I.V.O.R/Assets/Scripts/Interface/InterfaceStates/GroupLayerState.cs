namespace Interface.InterfaceStates
{
    public class GroupLayerState : LayerState
    {
        public GroupLayerState(InterfaceController contr, StateMachine sm) : base(contr, sm)
        {
        }
        

        public override void Enter()
        {
            //stateMachine.DefaultState = stateMachine.PreviousState;
            Selector.Instance.gameObject.SetActive(false);
            controller.GroupInfoLayer.SetActive(true);
            controller.MainInfoPanelLayer.SetActive(true);
        }

        public override void Exit()
        {
            Selector.Instance.gameObject.SetActive(true);
            controller.GroupInfoLayer.SetActive(false);
            controller.MainInfoPanelLayer.SetActive(false);
        }
    }
}