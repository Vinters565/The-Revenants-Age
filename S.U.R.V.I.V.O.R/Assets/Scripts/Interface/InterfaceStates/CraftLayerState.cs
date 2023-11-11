namespace Interface.InterfaceStates
{
    public class CraftLayerState : LayerState
    {
        public CraftLayerState(InterfaceController contr, StateMachine sm) : base(contr, sm)
        {
        }

        public override void Enter()
        {
            //stateMachine.DefaultState = stateMachine.PreviousState;
            Selector.Instance.gameObject.SetActive(false);
            controller.CraftLayer.SetActive(true);
            controller.MainInfoPanelLayer.SetActive(true);
        }

        public override void Exit()
        {
            Selector.Instance.gameObject.SetActive(true);
            controller.CraftLayer.SetActive(false);
            controller.MainInfoPanelLayer.SetActive(false);
        }
    }
}