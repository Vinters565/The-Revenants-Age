using Interface.PlayerInfoLayer;
using UnityEngine;

namespace Interface.InterfaceStates
{
    public class CharacterLayerState : LayerState
    {
        private readonly GameObject playerLayerObj;

        public CharacterLayerState(InterfaceController contr, StateMachine sm, PlayerLayerLogic obj)
            : base(contr, sm)
        {
            playerLayerObj = obj.gameObject;
        }

        public override void Enter()
        {
            //stateMachine.DefaultState = contr.CharactersState;
            Selector.Instance.gameObject.SetActive(false);
            playerLayerObj.SetActive(true);
            controller.MainInfoPanelLayer.SetActive(true);
            controller.CharactersButtonsLayer.SetActive(true);
        }

        public override void Exit()
        {
            Selector.Instance.gameObject.SetActive(true);
            controller.MainInfoPanelLayer.SetActive(false);
            playerLayerObj.SetActive(false);
            controller.CharactersButtonsLayer.SetActive(false);
        }
    }
}