﻿using MyGame.Game.StateMachine.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.StateMachine
{
    internal class StateMachine<TState> : IFiniteStateMachine<TState>
    {
        protected readonly IDictionary<TState, ICollection<KeyValuePair<ICollection<ICondition>, TState>>> _transitionConditions;

        protected readonly HashSet<string> _triggers = new();
        protected readonly Dictionary<string, object> _parameters = new();

        private TState _state;
        public virtual TState State
        {
            get => _state;
            set
            {
                var prevState = _state;
                _state = value;
                _triggers.Clear();
                OnStateChange(prevState, _state);
            }
        }

        public event EventHandler<TransitionEventArgs<TState>> StateChanged;

        public StateMachine(IDictionary<TState, ICollection<KeyValuePair<ICollection<ICondition>, TState>>> transitionConditions, TState initialState)
        {
            if (!transitionConditions.Keys.Any(x => Equals(initialState, x)))
            {
                throw new ArgumentException($"{nameof(initialState)} must be in state collection");
            }
            _transitionConditions = transitionConditions;
            _state = initialState;
        }

        public virtual void SetTrigger(string trigger)
        {
            _triggers.Add(trigger);
            HandleStateChange();
        }

        public bool ContainTrigger(string trigger) => _triggers.Contains(trigger);

        public virtual void RemoveTrigger(string trigger)
        {
            _triggers.Remove(trigger);
        }

        public virtual void SetParameter<T>(string name, T value)
        {
            _parameters[name] = value;
            HandleStateChange();
        }

        public T GetParameter<T>(string name)
        {
            if (_parameters.TryGetValue(name, out var value))
            {
                return (T)value;
            }
            return default;
        }

        public virtual void RemoveParameter(string parameter)
        {
            _parameters.Remove(parameter);
        }

        private void OnStateChange(TState prevState, TState currState)
        {
            StateChanged?.Invoke(this, new TransitionEventArgs<TState>(prevState, currState));
        }

        protected bool HandleStateChange()
        {
            if (_transitionConditions.TryGetValue(State, out var transitions))
            {
                foreach (var transition in transitions)
                {
                    bool allPassed = transition.Key.All(condition =>
                    {
                        if (condition.IsTrigger)
                        {
                            return _triggers.Contains(condition.Name);
                        }
                        else if (_parameters.TryGetValue(condition.Name, out var parameter))
                        {
                            return condition.CheckCondition(parameter);
                        }
                        return false;
                    });

                    if (allPassed)
                    {
                        State = transition.Value;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
