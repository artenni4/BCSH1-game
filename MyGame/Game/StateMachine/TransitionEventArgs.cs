namespace MyGame.Game.StateMachine
{
    internal class TransitionEventArgs<TState> : EventArgs
    {
        /// <summary>
        /// State that was preceeding current state
        /// </summary>
        public TState PreviousState { get; }

        /// <summary>
        /// Current state of FSM
        /// </summary>
        public TState CurrentState { get; }

        public TransitionEventArgs(TState previousState, TState currentState)
        {
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }
}