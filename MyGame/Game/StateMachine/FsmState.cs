using System;

namespace MyGame.Game.StateMachine
{
    internal class FsmState<T>
    {
        private readonly Dictionary<T, ICollection<Action<TransitionInfo<T>>>> _stateOnEnter = new();
        private readonly ICollection<Action<TransitionInfo<T>>> _globalOnEnter = new List<Action<TransitionInfo<T>>>();

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
                if (_stateOnEnter.TryGetValue(_value, out var actions))
                {
                    foreach (var onEnter in actions)
                    {
                        onEnter(transition);
                    }
                }
                foreach (var onEnter in _globalOnEnter)
                {
                    onEnter(transition);
                }
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

        public void OnEnter(T state, Action<TransitionInfo<T>> action)
        {
            if (_stateOnEnter.TryGetValue(state, out var actions))
            {
                actions.Add(action);
            }
            else
            {
                _stateOnEnter.Add(state, new List<Action<TransitionInfo<T>>> { action });
            }
        }

        public void GlobalOnEnter(Action<TransitionInfo<T>> action)
        {
            _globalOnEnter.Add(action);
        }
    }
}