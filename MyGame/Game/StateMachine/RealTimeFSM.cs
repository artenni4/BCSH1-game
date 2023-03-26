using MyGame.Game.StateMachine;
using MyGame.Game.StateMachine.Conditions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace MyGame.Game.StateMachine
{
    /// <summary>
    /// General purpose finite state machine (FSM) class
    /// </summary>
    internal class RealTimeFSM<TState> : StateMachine<TState>, IRealTimeFSM<TState> where TState : IRealTimeState
    {
        public RealTimeFSM(IDictionary<TState, ICollection<KeyValuePair<ICollection<ICondition>, TState>>> transitionConditions, TState initialState)
            : base(transitionConditions, initialState) { }

        public TimeSpan StateSetTime { get; private set; }

        public override TState State
        { 
            get => base.State;
            set
            {
                base.State = value;
                StateSetTime = TimeSpan.Zero;
            }
        }

        public override void SetParameter<T>(string name, T value)
        {
            _parameters[name] = value;
            if (!State.IsInterruptible && StateSetTime < State.Duration)
            {
                return;
            }
            HandleStateChange();
        }

        public override void SetTrigger(string trigger)
        {
            _triggers.Add(trigger);
            if (!State.IsInterruptible && StateSetTime < State.Duration)
            {
                return;
            }
            HandleStateChange();
        }

        public void Update(TimeSpan elapsedTime)
        {
            StateSetTime += elapsedTime;
            if (!State.IsInterruptible && StateSetTime < State.Duration)
            {
                return;
            }
            HandleStateChange();
            //if (StateSetTime > State.Duration) // loop state set time if is bigger than duration 
            //{
            //    StateSetTime = TimeSpan.FromTicks(StateSetTime.Ticks % State.Duration.Ticks);
            //}
        }
    }
}
