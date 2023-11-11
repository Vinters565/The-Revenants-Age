namespace Interface.InterfaceStates
{
    public abstract class LayerState: State
    {
        protected InterfaceController controller;

        protected LayerState(InterfaceController controller, StateMachine stateMachine)
        {
            this.controller = controller;
            this.StateMachine = stateMachine;
        }
        
        
    }
}