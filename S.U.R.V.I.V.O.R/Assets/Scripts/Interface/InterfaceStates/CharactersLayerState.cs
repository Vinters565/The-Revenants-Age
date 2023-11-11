namespace Interface.InterfaceStates
{
    public class CharactersLayerState : LayerState
    {
        public CharactersLayerState(InterfaceController contr, StateMachine sm) : base(contr, sm)
        {
        }

        public override void Enter()
        {
            //stateMachine.DefaultState = contr.NothingState;
            controller.CharactersButtonsLayer.SetActive(true);
            controller.MainInfoPanelLayer.SetActive(true);
            controller.GroupButtonsLayer.SetActive(true);
            CameraController.Instance.IsActive = true; 
            MinimapController.Instance.isActive = true;
        }

        public override void Exit()
        {
            controller.CharactersButtonsLayer.SetActive(false);
            controller.MainInfoPanelLayer.SetActive(false);
            controller.GroupButtonsLayer.SetActive(false);
            CameraController.Instance.IsActive = false; 
            MinimapController.Instance.isActive = false;
        }
    }
}