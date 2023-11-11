public class StateMachine
{
    public bool IsLock { get; set; }
    public State CurrentState { get; private set; }
    public State PreviousState { get; private set; }
    public State DefaultState { get; set; }

    public void Initialize(State startingState, State defaultState = null)
    {
        CurrentState = startingState;
        DefaultState = defaultState;
        startingState.Enter();
    }

    public void ChangeState(State newState)
    {
        if (IsLock) return;
        PreviousState = CurrentState;
        PreviousState.Exit();
        if (DefaultState != null && CurrentState == newState)
        {
            CurrentState = DefaultState;
        }
        else
        {
             CurrentState = newState;
        }

        CurrentState.Enter();
    }
}