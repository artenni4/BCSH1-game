using System;

namespace MyGame.Game.StateMachine
{
    internal class FsmState<T>
    {
        public event EventHandler<TransitionInfo<T>> OnStateChanged;

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                var prevState = _value;
                _value = value;
                var transition = new TransitionInfo<T>(prevState, _value);
                timeSet = TimeSpan.Zero;

                OnStateChanged?.Invoke(this, transition);
            }
        }


        private TimeSpan timeSet;
        public TimeSpan TimeSet => timeSet;

        public FsmState()
        {
        }

        public FsmState(T value)
        {
            Value = value;
        }

        public void Update(TimeSpan timeSpan)
        {
            timeSet += timeSpan;
        }

        public static implicit operator FsmState<T>(T value)
        {
            return new FsmState<T>(value);
        }
    }
}