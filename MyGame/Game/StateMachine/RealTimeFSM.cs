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

        public void Update(TimeSpan elapsedTime)
        {
            StateSetTime += elapsedTime;
            if (!State.IsInterruptible && State.Duration < StateSetTime)
            {
                return;
            }

            HandleStateChange();
        }
    }
}
