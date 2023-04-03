using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.StateMachine
{
    internal interface IRealTimeFSM<TState> : IFiniteStateMachine<TState> where TState : IRealTimeState
    {
        event EventHandler StateCycleDone;
        TimeSpan StateCycleTime { get; }
        TimeSpan StateSetTime { get; }
        void Update(TimeSpan elapsedTime);
    }
}
