using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.StateMachine
{
    internal class StateMachine<TState, TTrigger>
    {
        private readonly Dictionary<TState, ICollection<KeyValuePair<TTrigger, TState>>> _triggerTransitions = new();
        private readonly Dictionary<TState, KeyValuePair<TimeSpan, TState>> _timeTransitions = new();
        private readonly Dictionary<TState, Action<TransitionInfo>> _stateOnEnter = new();

        private TState _state;
        private TimeSpan stateSet;
        public TState State
        { 
            get => _state; 
            private set 
            { 
                var prevState = _state;
                _state = value;
                stateSet = TimeSpan.Zero;
                if (_stateOnEnter.TryGetValue(_state, out var onEnter))
                {
                    onEnter(new TransitionInfo(prevState, _state));
                }
            }
        }

        private StateMachine(TState state)
        {
            _state = state;
        }

        public void Trigger(TTrigger trigger)
        {
            var prevState = State;
            bool changed = false; // ensure only one transition is bound to the trigger
            if (_triggerTransitions.TryGetValue(State, out var transitions))
            {
                foreach (var state in transitions)
                {
                    if (Equals(state.Key, trigger))
                    {
                        if (changed) throw new Exception("More than one transition is not allowed for one trigger");

                        State = state.Value;
                        changed = true;
                    }
                }
            }
        }

        public void Update(TimeSpan timeSpan)
        {
            stateSet += timeSpan;
            if (_timeTransitions.TryGetValue(State, out var transition) && stateSet >= transition.Key)
            {
                var prevState = State;
                State = transition.Value;
            }
        }

        public class TransitionInfo
        {
            public TState PreviousState { get; }
            public TState CurrentState { get; }

            public TransitionInfo(TState previousState, TState currentState)
            {
                PreviousState = previousState;
                CurrentState = currentState;
            }
        }

        public class Builder : IStateBuilder, ITransitionBuilder
        {
            private TState _currentState;
            private TState _nextState;
            private readonly StateMachine<TState, TTrigger> _machine;

            public Builder(TState initialState)
            {
                _machine = new(initialState);
            }

            public StateMachine<TState, TTrigger> Build()
            {
                return _machine;
            }

            public IStateBuilder State(TState state)
            {
                _currentState = state;
                return this;
            }

            public ITransitionBuilder TransitionTo(TState nextState)
            {
                _nextState = nextState;
                return this;
            }

            public Builder OnEnter(Action<TransitionInfo> onEnter)
            {
                _machine._stateOnEnter.Add(_currentState, onEnter);
                return this;
            }

            public Builder On(TTrigger trigger)
            {
                if (_machine._triggerTransitions.TryGetValue(_currentState, out var collection))
                {
                    collection.Add(new(trigger, _nextState));
                }
                else
                {
                    _machine._triggerTransitions.Add(_currentState, new List<KeyValuePair<TTrigger, TState>>() { new(trigger, _nextState) });
                }
                return this;
            }

            public Builder After(TimeSpan timeSpan)
            {
                _machine._timeTransitions[_currentState] = new(timeSpan, _nextState);
                return this;
            }
        }

        public interface IStateBuilder
        {
            Builder OnEnter(Action<TransitionInfo> onEnter);
            ITransitionBuilder TransitionTo(TState nextState);
        }

        public interface ITransitionBuilder
        {
            Builder On(TTrigger trigger);
            Builder After(TimeSpan timeSpan);
        }
    }
}
