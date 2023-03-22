using MyGame.Game.StateMachine.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.StateMachine
{
    internal class StateMachineBuilder<TState>
    {
        private TState _currentState;
        private TState _nextState;
        public Dictionary<TState, ICollection<KeyValuePair<ICollection<ICondition>, TState>>> TransitionConditions { get; } = new();

        /// <summary>
        /// Sets building state
        /// </summary>
        /// <param name="state">State instance</param>
        /// <returns>State builder</returns>
        public StateMachineBuilder<TState> State(TState state)
        {
            _currentState = state;
            TransitionConditions[_currentState] = new List<KeyValuePair<ICollection<ICondition>, TState>>();
            return this;
        }

        public StateMachineBuilder<TState> TransitionTo(TState nextState)
        {
            _nextState = nextState;
            var transitions = TransitionConditions[_currentState];
            if (!transitions.Where(t => Equals(t.Value, _nextState)).Any())
            {
                transitions.Add(new KeyValuePair<ICollection<ICondition>, TState>(new List<ICondition>(), _nextState));
            }
            return this;
        }

        private ICollection<ICondition> GetConditionsCollection()
        {
            return TransitionConditions[_currentState].First(t => Equals(t.Value, _nextState)).Key;
        }

        public StateMachineBuilder<TState> OnTrigger(string trigger)
        {
            GetConditionsCollection().Add(new TriggerCondition(trigger));
            return this;
        }

        public StateMachineBuilder<TState> OnEquals<T>(string name, T value)
        {
            GetConditionsCollection().Add(new EqualsCondition<T>(name, value));
            return this;
        }

        public StateMachineBuilder<TState> OnLessThan<T>(string name, T value) where T : IComparable
        {
            GetConditionsCollection().Add(new LessThanCondition<T>(name, value));
            return this;
        }

        public StateMachineBuilder<TState> OnGreaterThan<T>(string name, T value) where T : IComparable
        {
            GetConditionsCollection().Add(new GreaterThanCondition<T>(name, value));
            return this;
        }
    }
}
