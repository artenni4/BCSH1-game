namespace MyGame.Game.StateMachine
{
    internal class TransitionInfo<T>
    {
        /// <summary>
        /// State that was preceeding current state
        /// </summary>
        public T PreviousState { get; }

        /// <summary>
        /// Current state of FSM
        /// </summary>
        public T CurrentState { get; }

        public TransitionInfo(T previousState, T currentState)
        {
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }
}