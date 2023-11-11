
namespace Interface.InterfaceStates
{
    public class NothingLayerState: LayerState
    {
        public NothingLayerState(InterfaceController contr, StateMachine sm) : base(contr, sm)
        {
        }

        public override void Enter()
        {
            controller.MainInfoPanelLayer.SetActive(true);
            controller.GroupButtonsLayer.SetActive(true);
            CameraController.Instance.IsActive = true;
            MinimapController.Instance.isActive = true;
        }

        public override void Exit()
        {
            controller.MainInfoPanelLayer.SetActive(false);
            controller.GroupButtonsLayer.SetActive(false);
            CameraController.Instance.IsActive = false;
            MinimapController.Instance.isActive = false;
        }
    }
}