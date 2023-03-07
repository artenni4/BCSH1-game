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
        


        //private TState _state;
        //private TimeSpan stateSet;
        ///// <summary>
        ///// Current state of FSM
        ///// </summary>
        //public TState State
        //{
        //    get => _state;
        //    private set
        //    {
        //        var prevState = _state;
        //        _state = value;
        //        stateSet = TimeSpan.Zero;
        //        if (_stateOnEnter.TryGetValue(_state, out var onEnter))
        //        {
        //            onEnter(new TransitionInfo<TState>(prevState, _state));
        //        }
        //    }
        //}

        /// <summary>
        /// Trigger some state change
        /// </summary>
        /// <param name="trigger">Tigger for change</param>
        /// <exception cref="Exception">Exception thrown if trigger is set to change state to more than one defined state</exception>
        public void Trigger(FsmState<TState> state, TTrigger trigger)
        {
            if (_triggerTransitions.TryGetValue(state.Value, out var transitions) && transitions.TryGetValue(trigger, out var nextState))
            {
                state.Value = nextState;
            }
        }

        /// <summary>
        /// Updates FSM with time span, used to transit between states with time trigger
        /// </summary>
        /// <param name="timeSpan"></param>
        public void Update(FsmState<TState> state, TimeSpan timeSpan)
        {
            // handle all transitions that are triggered by time
            state.Update(timeSpan);
            if (_timeTransitions.TryGetValue(state.Value, out var transition) && state.TimeSet >= transition.Key)
            {
                state.Value = transition.Value;
            }
        }

        public class Builder : IStateBuilder, ITransitionBuilder
        {
            private TState _currentState;
            private TState _nextState;
            private readonly StateMachine<TState, TTrigger> _machine;

            public Builder()
            {
                _machine = new();
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

            public ITransitionBuilder TransitionTo(TState nextState)
            {
                _nextState = nextState;
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
            ITransitionBuilder TransitionTo(TState nextState);
        }

        public interface ITransitionBuilder
        {
            Builder OnTrigger(TTrigger trigger);
            Builder After(TimeSpan timeSpan);
        }
    }
}
