using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.StateMachine
{
    internal interface IRealTimeFSM<TState> : IFiniteStateMachine<TState> where TState : IRealTimeState
    {
        TimeSpan StateSetTime { get; }
        void Update(TimeSpan elapsedTime);
    }
}
