using Interface.Menu.ForConsole;

namespace Interface.InterfaceStates
{
    public class GameConsoleLayerState: LayerState
    {
        public GameConsoleLayerState(InterfaceController controller, StateMachine stateMachine)
            : base(controller, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            controller.MainInfoPanelLayer.SetActive(true);
            controller.GroupButtonsLayer.SetActive(true);
            GameConsole.Instance.gameObject.SetActive(true);
            CameraController.Instance.IsActive = false;
            Selector.Instance.gameObject.SetActive(false);
        }

        public override void Exit()
        {
            base.Exit();
            controller.MainInfoPanelLayer.SetActive(false);
            controller.GroupButtonsLayer.SetActive(false);
            GameConsole.Instance.gameObject.SetActive(false);
            CameraController.Instance.IsActive = true;
            Selector.Instance.gameObject.SetActive(true);
        }
    }
}