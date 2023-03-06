using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.StateMachine
{
    /// <summary>
    /// General finite state machine (FSM) class for different purposes
    /// </summary>
    internal class StateMachine<TState, TTrigger>
    {
        private readonly Dictionary<TState, Dictionary<TTrigger, TState>> _triggerTransitions = new();
        private readonly Dictionary<TState, KeyValuePair<TimeSpan, TState>> _timeTransitions = new();
        private readonly Dictionary<TState, Action<TransitionInfo>> _stateOnEnter = new(); // TODO: allow list of actions on enter (make list)
        private readonly Dictionary<TState, Action<TState, TimeSpan>> _stateOnUpdate = new();


        private TState _state;
        private TimeSpan stateSet;
        /// <summary>
        /// Current state of FSM
        /// </summary>
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

        /// <summary>
        /// Trigger some state change
        /// </summary>
        /// <param name="trigger">Tigger for change</param>
        /// <exception cref="Exception">Exception thrown if trigger is set to change state to more than one defined state</exception>
        public void Trigger(TTrigger trigger)
        {
            if (_triggerTransitions.TryGetValue(State, out var transitions) && transitions.TryGetValue(trigger, out var nextState))
            {
                State = nextState;
            }
        }

        /// <summary>
        /// Updates FSM with time span, used to transit between states with time trigger
        /// </summary>
        /// <param name="timeSpan"></param>
        public void Update(TimeSpan timeSpan)
        {
            // handle all transitions that are triggered by time
            stateSet += timeSpan;
            if (_timeTransitions.TryGetValue(State, out var transition) && stateSet >= transition.Key)
            {
                State = transition.Value;
            }

            // update all states that do something on update
            if (_stateOnUpdate.TryGetValue(State, out var onUpdate))
            {
                onUpdate(State, timeSpan);
            }
        }

        public class TransitionInfo
        {
            /// <summary>
            /// State that was preceeding current state
            /// </summary>
            public TState PreviousState { get; }

            /// <summary>
            /// Current state of FSM
            /// </summary>
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

            /// <summary>
            /// Rerurn built FSM
            /// </summary>
            public StateMachine<TState, TTrigger> Build()
            {
                return _machine;
            }

            /// <summary>
            /// Sets building state
            /// </summary>
            /// <param name="state">State instance</param>
            /// <returns>State builder</returns>
            public IStateBuilder State(TState state)
            {
                _currentState = state;
                return this;
            }

            public Builder DeadEnd()
            {
                _machine._triggerTransitions.Add(_currentState, new Dictionary<TTrigger, TState>());
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

            public Builder GlobalOnEnter(Action<TransitionInfo> onEnter)
            {
                foreach (var state in _machine._triggerTransitions.Keys.Concat(_machine._timeTransitions.Keys))
                {
                    _machine._stateOnEnter.Add(state, onEnter);
                }
                return this;
            }

            public Builder OnUpdate(Action<TState, TimeSpan> onUpdate)
            {
                _machine._stateOnUpdate.Add(_currentState, onUpdate);
                return this;
            }

            public Builder GlobalOnUpdate(Action<TState, TimeSpan> onUpdate)
            {
                foreach (var state in _machine._triggerTransitions.Keys.Concat(_machine._timeTransitions.Keys))
                {
                    _machine._stateOnUpdate.Add(state, onUpdate);
                }
                return this;
            }

            public Builder OnTrigger(TTrigger trigger)
            {
                if (_machine._triggerTransitions.TryGetValue(_currentState, out var collection))
                {
                    collection[trigger] = _nextState;
                }
                else
                {
                    _machine._triggerTransitions.Add(_currentState, new Dictionary<TTrigger, TState>() { { trigger, _nextState } });
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
            Builder DeadEnd();
        }

        public interface ITransitionBuilder
        {
            Builder OnTrigger(TTrigger trigger);
            Builder After(TimeSpan timeSpan);
        }
    }
}
