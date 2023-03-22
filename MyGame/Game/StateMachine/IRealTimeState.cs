using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.StateMachine
{
    internal interface IRealTimeState
    {
        bool IsInterruptible { get; }
        TimeSpan Duration { get; }
    }
}
